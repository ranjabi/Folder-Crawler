
using System;
using System.IO;
using System.Collections;
using Microsoft.Msagl;
using Microsoft.Msagl.Splines;
namespace FolderCrawling

{


    public partial class Form1 : Form
    {
        //string selectedPath = GlobalVar.selectedPath;
        //bool isFolderChoosen = GlobalVar.isFolderChosen;
        public Form1()
        {
            InitializeComponent();
            
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
            } else
            {
                MessageBox.Show("Choose Folder First!");
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

    }

    public static class GlobalVar
    {
        public static string selectedPath = "";
        public static bool isFolderChoosen = false;
        public static string searchVal = "";
    }

    class Node
    {
        public string Name;
        public List<Node> Children = new List<Node>();
        public Node(string name, List<Node> children)
        {
            this.Name = name;
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
                visited.Add(temp.Name, false);
                initVisited(temp, visited);
            }
            
        }
        public static string DFS(string searchVal, string vertex, Dictionary<string, bool> visited, Node tree, Microsoft.Msagl.Drawing.Graph graph)
        {
            //graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            //MessageBox.Show(graph.FindNode(vertex).Attr.Id);
            //if (graph.FindNode(vertex).Attr.Id == searchVal)
            //{
            //    graph.FindNode(vertex).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
            //}
            if (vertex == searchVal)
            {
                graph.FindNode(searchVal).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
                return tree.Name;
            }

            visited[vertex] = true;
            graph.FindNode(vertex).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            foreach(Node temp in tree.Children)
            {
                if (!visited[temp.Name])
                {
                    if (temp.Name == searchVal)
                    {
                        graph.FindNode(searchVal).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
                        return temp.Name;
                    }
                    string find = DFS(searchVal, temp.Name, visited, temp, graph);
                    if (find == searchVal)
                    {
                        graph.FindNode(searchVal).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
                        return find;
                    }
                }
            }
            return null;

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
                Node temp = new Node(parentFolderName, new List<Node>());
                tree.Children.Add(temp);
                graph.AddEdge(root, parentFolderName);
                findDir(dir, graph, temp);

            }

            foreach (var file in files)
            {
                //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                parentFolderName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                // Console.WriteLine($"dir: {dir}");
                Node temp = new Node(parentFolderName, new List<Node>());

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

            Node tree = new Node(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), new List<Node>());

            findDir(docPath, graph, tree);

            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            initVisited(tree, visited);
            //foreach (KeyValuePair<string, bool> temp in visited)
            //{
            //    Console.WriteLine("Key: {0}, Value: {1}", temp.Key, temp.Value);
            //}
            //DFS(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), GlobalVar.searchVal, graph);
            DFS(GlobalVar.searchVal, tree.Name, visited, tree, graph);

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