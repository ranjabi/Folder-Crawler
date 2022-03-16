using System;
using System.IO;
using System.Collections;
using Microsoft.Msagl;
using Microsoft.Msagl.Splines;
using System;
using System.Text.RegularExpressions;
namespace FolderCrawling

{


    public partial class Form1 : Form
    {
        //string selectedPath = GlobalVar.selectedPath;
        //bool isFolderChoosen = GlobalVar.isFolderChosen;
        public Form1()
        {
            InitializeComponent();
            //linkLabel1.Location = new System.Drawing.Point(34, 56);
            linkLabel1.Size = new System.Drawing.Size(224, 16);
            linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 8);
            linkLabel1.Text = "Register Online.  \nVisit Microsoft. \n Visit MSN.";
            linkLabel1.Links[0].LinkData = "C:\\";
            linkLabel1.Links.Add(24, 9, "www.microsoft.com");
            linkLabel1.Links.Add(42, 3, "www.msn.com");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                GlobalVar.selectedPath = dialog.SelectedPath;
                label4.Text = GlobalVar.selectedPath;
                GlobalVar.isFolderChoosen = true;
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            if (GlobalVar.isFolderChoosen)
            {
                string searchingPath = label4.Text + "\\...\\" + textBox1.Text;
                label8.Text = searchingPath;
                GlobalVar.searchVal = textBox1.Text;
                ViewerSample.Launch();
                foreach (string path in GlobalVar.foundPath)
                {
                    label9.Text += (path + "\n");
                }
                //label9.Text = "kambing";
            }
            else
            {
                MessageBox.Show("Choose Folder First!");
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVar.allOccurence = true;


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            System.Diagnostics.Process.Start("explorer.exe"+ target + "" + "\"");
        }
    }

    public static class GlobalVar
    {
        public static string selectedPath = "";
        public static bool isFolderChoosen = false;
        public static string searchVal = "";
        public static bool allOccurence;
        public static List<string> foundPath = new List<string>();
    }

    class Node
    {
        public string Name;
        public string path;
        public List<Node> Children = new List<Node>();
        public Node(string name, string path, List<Node> children)
        {
            this.Name = name;
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

    class ViewerSample
    {
        public static void initVisited(Node node, Dictionary<string, bool> visited)
        {
            foreach (Node temp in node.Children)
            {
                visited.Add(temp.path, false);
                initVisited(temp, visited);
            }

        }
        public static String DFS(List<string> foundPath, bool allOccurence, string searchVal, Dictionary<string, bool> visited, Node tree, Microsoft.Msagl.Drawing.Graph graph)
        {
            // Using regex to find searchVal followed by whitespace at the end of the word
            searchVal =  @"\b" + searchVal + @"\s*";


            graph.FindNode(tree.Name).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            visited[tree.path] = true;

            if (Regex.IsMatch(tree.Name, searchVal))
            {
                graph.FindNode(tree.Name).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
                foundPath.Add(tree.path);
                MessageBox.Show(tree.path + "1");
                if (!allOccurence)
                {
                    return tree.path;
                }
            }
            
            foreach (Node temp in tree.Children)
            {
                if (!visited[temp.path])
                {
                    //if (Regex.IsMatch(temp.Name, searchVal))
                    //{

                    //    graph.FindNode(temp.Name).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
                    //    foundPath.Append(tree.path);
                    //    MessageBox.Show(tree.path + "2");
                    //    if (!allOccurence)
                    //    {
                    //        return temp.path;
                    //    }
                    //}
                    String find = DFS(foundPath, allOccurence, searchVal, visited, temp, graph); //
                    if (Regex.IsMatch(find.Substring(find.LastIndexOf(Path.DirectorySeparatorChar) + 1), searchVal))
                    {
                        graph.FindNode(find.Substring(find.LastIndexOf(Path.DirectorySeparatorChar) + 1)).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
                        //foundPath.Append(tree.path);
                        MessageBox.Show(tree.path + "3");
                        if (!allOccurence)
                        {
                            return find;
                        }
                    }
                }
            }
            return "";

        }
        public static void findDir(string docPath, Microsoft.Msagl.Drawing.Graph graph, Node tree)
        {
            //Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            // Enumerasi docPath
            string root = docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            List<string> dirs = new List<string>(Directory.EnumerateDirectories(docPath));
            List<string> files = new List<string>(Directory.EnumerateFiles(docPath));

            string parentFolderName = "";

            foreach (var dir in dirs)
            {
                //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                parentFolderName = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                // Console.WriteLine($"dir: {dir}");
                if (graph.FindNode(parentFolderName) != null)
                {
                    parentFolderName += " ";
                }
                Node temp = new Node(parentFolderName, dir, new List<Node>());
                tree.Children.Add(temp);
                //string id = graph.AddEdge(root, parentFolderName).Attr.Id;
                //MessageBox.Show(id);

                graph.AddEdge(root, parentFolderName);
                //graph.RemoveEdge(temps);
                //graph.RemoveEdge(graph.EdgeById)
                findDir(dir, graph, temp);

            }

            foreach (var file in files)
            {
                //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                parentFolderName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                // Console.WriteLine($"dir: {dir}");
                Node temp = new Node(parentFolderName, file, new List<Node>());

                tree.Children.Add(temp);
                graph.AddEdge(root, parentFolderName);

            }
            // return parentFolderName;
        }
        public static void Launch()
        {


            //create a form 
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");


            //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string docPath = GlobalVar.selectedPath;

            Node tree = new Node(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), docPath, new List<Node>());

            findDir(docPath, graph, tree);

            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            initVisited(tree, visited);
            //foreach (KeyValuePair<string, bool> temp in visited)
            //{
            //    Console.WriteLine("Key: {0}, Value: {1}", temp.Key, temp.Value);
            //}
            //DFS(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), GlobalVar.searchVal, graph);
            DFS(GlobalVar.foundPath, GlobalVar.allOccurence, GlobalVar.searchVal, visited, tree, graph);

            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.ShowDialog();
        }
    }
}