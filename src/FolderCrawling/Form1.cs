using System;
using System.IO;
using System.Collections;
using Microsoft.Msagl;
using Microsoft.Msagl.Splines;
using System.Text;
using System.Text.RegularExpressions;
using Edge = Microsoft.Msagl.Drawing.Edge;
using MsaglDraw = Microsoft.Msagl.Drawing;
using System.Threading.Tasks;
using MaterialSkin;
using MaterialSkin.Controls;

namespace FolderCrawling

{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            MaterialSkinManager materialSkin = MaterialSkinManager.Instance;
            materialSkin.AddFormToManage(this);
            materialSkin.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkin.ColorScheme = new ColorScheme(
                    Primary.Blue400, Primary.Blue500,
                    Primary.Blue500, Accent.LightBlue200,
                    TextShade.WHITE
                );

        }

        // delay used for visualization
        public static int delay = int.Parse(Program.form1.materialLabel5.Text);

        private void button1_Click(object sender, EventArgs e)
        // choose folder/file button
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // assign selected folder/file path
                GlobalVar.selectedPath = dialog.SelectedPath;
                materialLabel1.Text = GlobalVar.selectedPath;
                GlobalVar.isFolderChoosen = true;
            }
        }

        public static void updateGraph()
        // update graph panel to new condition
        {
            GlobalVar.viewer.Graph = GlobalVar.graph;
            Program.form1.panel1.SuspendLayout();
            GlobalVar.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            Program.form1.panel1.Controls.Add(GlobalVar.viewer);
            Program.form1.panel1.ResumeLayout();
        }

        private async void searchBtn_Click(object sender, EventArgs e)
        // search button
        {
            if (GlobalVar.isFolderChoosen)
            {

                GlobalVar.found = false;
                GlobalVar.graph = new Microsoft.Msagl.Drawing.Graph("graph");
                GlobalVar.foundPath.Clear();

                string searchingPath = materialLabel1.Text + "\\...\\" + materialSingleLineTextField1.Text;
                materialLabel7.Text = searchingPath;

                GlobalVar.searchVal = Regex.Escape(materialSingleLineTextField1.Text); // assign file/folder name to search value
                GlobalVar.edges.Clear();
                GlobalVar.visited.Clear();

                var task = GraphViewer.Launch();
                await Task.WhenAll(task);
                initializedFoundPath(); // add found path to linklabel

            }
            else
            {
                MessageBox.Show("Choose Folder First!");
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        // filename textbox
        {
            //textBox1.Clear();
            materialSingleLineTextField1.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        // bfs radiobutton
        {
            if (materialRadioButton1.Checked) {
                GlobalVar.method = "BFS";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        // dfs radiobutton
        {
            if (materialRadioButton2.Checked)
            {
                GlobalVar.method = "DFS";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        // all occurence button
        {
            if (GlobalVar.allOccurence == true)
            {
                GlobalVar.allOccurence = false;
            } else
            {
                GlobalVar.allOccurence = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        // open windows explorer on selected path
        {
            string path = "";
            int pathIndex = linkLabel1.Links.IndexOf(e.Link);
            path = GlobalVar.foundPath[pathIndex];

            FileAttributes attr = File.GetAttributes(path);
            if (!attr.HasFlag(FileAttributes.Directory))
            {
                path = Path.GetDirectoryName(path);
            }
            System.Diagnostics.Process.Start("explorer.exe", "\"" + path + "\"");
        }

        public void initializedFoundPath()
        // assign link to path text
        {
            linkLabel1.Links.Clear();
            linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            string allPath = "";
            foreach (string foundPath in GlobalVar.foundPath)
            {
                allPath += foundPath + "\n";
            }
            linkLabel1.Text = allPath;
            string dirFilePath = "";
            foreach (string foundPath in GlobalVar.foundPath)
            {
                linkLabel1.Links.Add(allPath.IndexOf(foundPath), foundPath.Length);
            }
        }

        class GraphViewer
        {
            public static void initVisited(Node node, Dictionary<string, bool> visited)
            // add all file and folder directory node status to visited dictionary and set it's value to false
            {
                foreach (Node temp in node.Children)
                {
                    visited.Add(temp.path, false);
                    initVisited(temp, visited);
                }
            }

            public static async Task DFS(List<string> foundPath, bool allOccurence, string searchVal, Dictionary<string, bool> visited, Node tree, Microsoft.Msagl.Drawing.Graph graph)
            // perform BFS
            {

                if (!allOccurence && GlobalVar.found)
                {
                    return;
                }

                // using regex to find searchVal followed by whitespace at the end of the word
                string tempSearchVal = @"^" + searchVal + @"\s*\b";
                //MessageBox.Show(tempSearchVal);

                // set visited node color to red
                graph.FindNode(tree.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;

                if (tree.prevPath != null)
                {
                    GlobalVar.edges[tree.prevPath.path + tree.path].Attr.Color = MsaglDraw.Color.Red;
                    Form1.updateGraph();
                    await Task.Delay(delay);
                }

                visited[tree.path] = true;

                // set found node and it's parent color to blue
                if (Regex.IsMatch(tree.Name, tempSearchVal))
                {
                    graph.FindNode(tree.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    colorPath(tree.prevPath, tree, "blue", graph);
                    foundPath.Add(tree.path); // add path to foundPath
                    Form1.updateGraph();
                    await Task.Delay(delay);
                    if (!allOccurence)
                    {
                        GlobalVar.found = true;
                        return;
                    }

                }

                foreach (Node temp in tree.Children)
                {
                    if (!visited[temp.path])
                    {
                        await DFS(foundPath, allOccurence, searchVal, visited, temp, graph);
                    }
                }
                return;

            }

            static async Task BFS(string path, Node startNode, string searchVal, Microsoft.Msagl.Drawing.Graph graph, bool allOccurence)
            // perform DFS
            {
                bool found = false;
                searchVal = @"^" + searchVal + @"\s*\b";

                //MessageBox.Show(searchVal);
                Queue<Node> queue = new Queue<Node>();
                GlobalVar.visited[startNode.path] = true;

                // set found node and it's parent color to blue
                if (Regex.IsMatch(startNode.Name, searchVal))
                {
                    graph.FindNode(startNode.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    Form1.updateGraph();
                    await Task.Delay(delay);
                    found = true;
                    GlobalVar.foundPath.Add(startNode.path);
                    if (!allOccurence)
                    {
                        return;
                    }
                }
                else
                {
                    graph.FindNode(startNode.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    Form1.updateGraph();
                    await Task.Delay(delay);
                }

                queue.Enqueue(startNode);

                // perform bfs on queue
                while ((queue.Count > 0) && !found)
                {
                    Node curNode = queue.Dequeue();
                    foreach (Node temp in curNode.Children)
                    {
                        //MessageBox.Show("foreach: " + temp.Name + "\n" + temp.path);
                        if (!GlobalVar.visited[temp.path])
                        {
                            GlobalVar.visited[temp.path] = true;
                            queue.Enqueue(temp);

                            GlobalVar.edges[temp.prevPath.path + temp.path].Attr.Color = MsaglDraw.Color.Red;
                            Form1.updateGraph();
                            await Task.Delay(delay);

                            // set found node and it's parent color to blue
                            if (Regex.Match(temp.Name, searchVal).Success)
                            {
                                //MessageBox.Show(temp.Name + "\n" + temp.path);
                                colorPath(curNode, temp, "blue", graph);
                                graph.FindNode(temp.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                                Form1.updateGraph();
                                await Task.Delay(500);
                                GlobalVar.foundPath.Add(temp.path);
                                if (!allOccurence)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                graph.FindNode(temp.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                                Form1.updateGraph();
                                await Task.Delay(delay);
                            }
                        }
                    }
                }
            }

            public static void colorPath(Node source, Node target, string color, Microsoft.Msagl.Drawing.Graph graph)
            // fill the edge color between source and target node to it's parent
            {
                if (target.prevPath != null)
                {
                    //MessageBox.Show(source.path + "," + target.path);
                    if (color == "red")
                    {
                        GlobalVar.edges[source.path + target.path].Attr.Color = MsaglDraw.Color.Red;
                        graph.FindNode(target.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                        graph.FindNode(source.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    }
                    else if (color == "blue")
                    {
                        GlobalVar.edges[source.path + target.path].Attr.Color = MsaglDraw.Color.Blue;
                        graph.FindNode(target.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                        graph.FindNode(source.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    }
                    colorPath(source.prevPath, target.prevPath, color, graph);
                }
            }

            static async Task findDir(string path, Microsoft.Msagl.Drawing.Graph graph, Node tree)
            // draw all file and folder directory to graph
            {
                
                string root = tree.Name;

                // enumerate all possible folder and file path
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(path));
                List<string> files = new List<string>(Directory.EnumerateFiles(path));

                string parentFolderName = "";
                if (System.IO.Directory.GetDirectories(path).Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        parentFolderName = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        if (graph.FindNode(parentFolderName) != null)
                        {
                            parentFolderName += " ";
                            //MessageBox.Show(root + parentFolderName);
                        }
                        Node temp = new Node(parentFolderName, tree, dir, new List<Node>());
                        //MessageBox.Show(parentFolderName + "," + dir + "msg node");
                        tree.Children.Add(temp);

                        Edge tempEdge = graph.AddEdge(root, parentFolderName);
                        Form1.updateGraph();
                        //MessageBox.Show(root + "," + parentFolderName);
                        GlobalVar.edges.Add(path + dir, tempEdge);
                        await findDir(temp.path, graph, temp);
                    }
                }

                foreach (var file in files)
                {
                    GlobalVar.parentFileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    if (graph.FindNode(GlobalVar.parentFileName) != null)
                    {
                        GlobalVar.addon += " ";
                        GlobalVar.parentFileName += GlobalVar.addon;
                        //MessageBox.Show(root + parentFolderName);
                    }

                    Node temp = new Node(GlobalVar.parentFileName, tree, file, new List<Node>());

                    tree.Children.Add(temp);
                    Edge tempEdge = graph.AddEdge(root, GlobalVar.parentFileName);
                    Form1.updateGraph();
                    GlobalVar.edges.Add(path + file, tempEdge);
                }
            }

            public static async Task Launch()
            {
                string path = GlobalVar.selectedPath;

                // create tree
                Node tree = new Node(path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1), null, path, new List<Node>());

                findDir(tree.path, GlobalVar.graph, tree);

                // add root to visited dictionary
                GlobalVar.visited.Add(tree.path, false);
                initVisited(tree, GlobalVar.visited);

                if (GlobalVar.method == "DFS")
                {
                    await DFS(GlobalVar.foundPath, GlobalVar.allOccurence, GlobalVar.searchVal, GlobalVar.visited, tree, GlobalVar.graph);
                }
                else if (GlobalVar.method == "BFS")
                {

                    await BFS(GlobalVar.selectedPath, tree, GlobalVar.searchVal, GlobalVar.graph, GlobalVar.allOccurence);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        // delay value
        {
            //label10.Text = trackBar1.Value.ToString(); 
            materialLabel5.Text = trackBar1.Value.ToString();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void materialRadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel5_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel7_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel6_Click(object sender, EventArgs e)
        {

        }
    }
}

public static class GlobalVar
    {
        public static string selectedPath = "";
        public static bool isFolderChoosen = false;
        public static string searchVal = "";
        public static bool allOccurence;
        public static string method;
        public static string addon = "";
        public static string parentFileName = "";
        public static List<string> foundPath = new List<string>();
        public static Dictionary<string, Edge> edges = new Dictionary<string, Edge>();
        public static Dictionary<string, bool> visited = new Dictionary<string, bool>();
        public static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        public static Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
        public static bool found = false;
}

class Node
    {
        public string Name;
        public Node prevPath;
        public string path;
        public List<Node> Children = new List<Node>();
        public Node(string name, Node prevPath, string path, List<Node> children)
        {
            this.Name = name;
            this.prevPath = prevPath;
            this.path = path;
            this.Children = new List<Node>();
        }
    }