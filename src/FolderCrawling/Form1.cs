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
namespace FolderCrawling

{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int delay = int.Parse(Program.form1.label10.Text);

        public Panel getPanel()
        {
            return this.panel1;
        }

        private void button1_Click(object sender, EventArgs e)
        // choose folder button
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                GlobalVar.selectedPath = dialog.SelectedPath;
                label4.Text = GlobalVar.selectedPath;
                GlobalVar.isFolderChoosen = true;
            }
        }

        async static void UseDelay()
        {
            await Task.Delay(1000); // wait for 1 second
        }

       public static void updateGraph()
        {
            //await Task.Delay(1000);
            GlobalVar.viewer.Graph = GlobalVar.graph;
            Program.form1.panel1.SuspendLayout();
            GlobalVar.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            Program.form1.panel1.Controls.Add(GlobalVar.viewer);
            Program.form1.panel1.ResumeLayout();
            ////show the form 
            //Program.form1.panel1.Show();
            
        }

        private async void searchBtn_Click(object sender, EventArgs e)
        // search button
        {
            if (GlobalVar.isFolderChoosen)
            {

                GlobalVar.found = false;
                GlobalVar.graph = new Microsoft.Msagl.Drawing.Graph("graph");
                GlobalVar.foundPath.Clear();
                //label9.Text = "Found Path: \n";

                string searchingPath = label4.Text + "\\...\\" + textBox1.Text;
                label8.Text = searchingPath;

                GlobalVar.searchVal = Regex.Escape(textBox1.Text);
                GlobalVar.edges.Clear();
                GlobalVar.visited.Clear();
                

                var task1 = GraphViewer.Launch();
                await Task.WhenAll(task1);
                initializedFoundPath();

                
                
                //foreach (string path in GlobalVar.foundPath)
                //{
                //    label9.Text += (path + "\n");
                //}
                //label9.Text = "kambing";
            }
            else
            {
                MessageBox.Show("Choose Folder First!");
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        // filename textbox
        {
            textBox1.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        // bfs radiobutton
        {
            if (radioButton1.Checked) {
                GlobalVar.method = "BFS";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        // dfs radiobutton
        {
            if (radioButton2.Checked)
            {
                GlobalVar.method = "DFS";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        // alloccurence button
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
        {
            string path = "";
            int pathIndex = linkLabel1.Links.IndexOf(e.Link);
            path = GlobalVar.foundPath[pathIndex];

            FileAttributes attr = File.GetAttributes(path);
            if (!attr.HasFlag(FileAttributes.Directory))
            {
                path = Path.GetDirectoryName(path);
            }

            //System.Diagnostics.Process.Start("Chrome.exe", path);
            System.Diagnostics.Process.Start("explorer.exe", "\"" + path + "\"");
        }

        public void initializedFoundPath()
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
            // add all file and folder directory to visited and set it's value to false
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


                graph.FindNode(tree.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                //Form1.updateGraph();
                //await Task.Delay(delay);
                //await Task.Delay(delay);

                if (tree.prevPath != null)
                {
                    GlobalVar.edges[tree.prevPath.path + tree.path].Attr.Color = MsaglDraw.Color.Red;
                    Form1.updateGraph();
                    await Task.Delay(delay);
                }

                visited[tree.path] = true;

                if (Regex.IsMatch(tree.Name, tempSearchVal))
                {
                    graph.FindNode(tree.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;

                    colorPath(tree.prevPath, tree, "blue", graph);
                    foundPath.Add(tree.path);
                    Form1.updateGraph();
                    await Task.Delay(delay);
                    if (!allOccurence)
                    {
                        GlobalVar.found = true;
                        return;
                    }
                    //MessageBox.Show(tree.path + "1");

                }

                foreach (Node temp in tree.Children)
                {
                    if (!visited[temp.path])
                    {
                        //Node find = 
                        await DFS(foundPath, allOccurence, searchVal, visited, temp, graph);
                        //
                        //if (Regex.IsMatch(find.Name, tempSearchVal))
                        //{
                        //    graph.FindNode(find.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                        //    Form1.updateGraph();
                        //    UseDelay();
                        //    //MessageBox.Show(tree.path + "\n" + temp.path + "\n" + find.path);
                        //    colorPath(tree, temp, "blue", graph);
                        //    foundPath.Append(tree.path);
                        //    //MessageBox.Show(tree.path + "3");
                        //if (!allOccurence)
                        //{
                        //    found = true;
                        //    break;
                        //}
                        //}
                    }
                }
                return;

            }

            static async Task BFS(string docPath, Node startNode, string searchVal, Microsoft.Msagl.Drawing.Graph graph, bool allOccurence)
            // perform DFS
            {
                bool found = false;
                searchVal = @"^" + searchVal + @"\s*\b";

                //MessageBox.Show(searchVal);
                Queue<Node> queue = new Queue<Node>();
                GlobalVar.visited[startNode.path] = true;

                if (Regex.IsMatch(startNode.Name, searchVal))
                {
                    graph.FindNode(startNode.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    Form1.updateGraph();
                    await Task.Delay(delay);
                    found = true;
                    GlobalVar.foundPath.Add(startNode.path);
                    //MessageBox.Show("ini regex pertama");
                    if (!allOccurence)
                    {
                        return;
                    }
                }
                else
                {
                    graph.FindNode(startNode.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    Form1.updateGraph();
                    await Task.Delay(500);
                }
                queue.Enqueue(startNode);

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
                            Form1.UseDelay();

                            GlobalVar.edges[temp.prevPath.path + temp.path].Attr.Color = MsaglDraw.Color.Red;
                            Form1.updateGraph();
                            await Task.Delay(delay);

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
                                //MessageBox.Show("break kelewat");
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
            // fill the edge color between source and target node
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

            static async Task findDir(string docPath, Microsoft.Msagl.Drawing.Graph graph, Node tree)
            // draw all file and folder directory to graph
            {
                // Enumerasi docPath
                string root = tree.Name;

                List<string> dirs = new List<string>(Directory.EnumerateDirectories(docPath));
                List<string> files = new List<string>(Directory.EnumerateFiles(docPath));

                string parentFolderName = "";


                if (System.IO.Directory.GetDirectories(docPath).Length > 0)
                {

                    foreach (var dir in dirs)
                    {
                        //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                        parentFolderName = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        // Console.WriteLine($"dir: {dir}");
                        if (graph.FindNode(parentFolderName) != null)
                        {
                            parentFolderName += " ";
                            //temp.path += "1"
                            //MessageBox.Show(root + parentFolderName);
                        }
                        Node temp = new Node(parentFolderName, tree, dir, new List<Node>());
                        //MessageBox.Show(parentFolderName + "," + dir + "msg node");
                        tree.Children.Add(temp);

                        Edge tempEdge = graph.AddEdge(root, parentFolderName);
                        Form1.updateGraph();
                        //await Task.Delay(500);
                        //MessageBox.Show(root + "," + parentFolderName);
                        GlobalVar.edges.Add(docPath + dir, tempEdge);
                        //Form1.updateGraph();
                        //Form1.upda
                        //graph.RemoveEdge(temps);
                        //graph.RemoveEdge(graph.EdgeById)

                        await findDir(temp.path, graph, temp);

                    }
                }

                foreach (var file in files)
                {
                    //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                    GlobalVar.parentFileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    if (graph.FindNode(GlobalVar.parentFileName) != null)
                    {
                        GlobalVar.addon += " ";
                        GlobalVar.parentFileName += GlobalVar.addon;
                        //temp.path += "1"
                        //MessageBox.Show(root + parentFolderName);
                    }

                    // Console.WriteLine($"dir: {dir}");
                    Node temp = new Node(GlobalVar.parentFileName, tree, file, new List<Node>());

                    tree.Children.Add(temp);
                    Edge tempEdge = graph.AddEdge(root, GlobalVar.parentFileName);
                    Form1.updateGraph();
                    //await Task.Delay(500);
                    GlobalVar.edges.Add(docPath + file, tempEdge);

                }
            }

            public static async Task Launch()
            {


                //create a form 
                //System.Windows.Forms.Form form = new System.Windows.Forms.Form();
                //create a viewer object 

                //create a graph object 



                //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string docPath = GlobalVar.selectedPath;

                Node tree = new Node(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), null, docPath, new List<Node>());

                findDir(tree.path, GlobalVar.graph, tree);

                GlobalVar.visited.Add(tree.path, false);
                initVisited(tree, GlobalVar.visited);
                ////////////////////////////
                if (GlobalVar.method == "DFS")
                {
                    await DFS(GlobalVar.foundPath, GlobalVar.allOccurence, GlobalVar.searchVal, GlobalVar.visited, tree, GlobalVar.graph);
                }
                else if (GlobalVar.method == "BFS")
                {

                    await BFS(GlobalVar.selectedPath, tree, GlobalVar.searchVal, GlobalVar.graph, GlobalVar.allOccurence);
                }

                //GlobalVar.edges["srcconfig"].Attr.Color = Microsoft.Msagl.Drawing.Color.Red;

                ////bind the graph to the viewer 
                //GlobalVar.viewer.Graph = GlobalVar.graph;
                ////associate the viewer with the form 
                //GlobalVar.viewer.Dock = System.Windows.Forms.DockStyle.Fill;


            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label10.Text = trackBar1.Value.ToString(); 
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

        public void PrintNode(string prefix)
        {
            //Console.WriteLine("{0} + {1}", prefix, this.Name);
            foreach (Node n in Children)
                if (Children.IndexOf(n) == Children.Count - 1)
                    n.PrintNode(prefix + "    ");
                else
                    n.PrintNode(prefix + "   |");
        }
        public Node searchNode(string val, Node node)
        {
            if (node.Name == val)
            {
                // Console.WriteLine($"node key: {node.key}");
                return node;
            }
            else
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    searchNode(val, node.Children[i]);
                }
            }
            return node;
        }

    }