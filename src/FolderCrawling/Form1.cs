
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

    public class Node
    {
        public string key;
        public Node child;
        public Node(string val)
        {
            key = val;
            child = null;
        }
    }

    public class Tree
    {
        public Node root;
        public Tree(string key)
        {
            root = new Node(key);
        }
        public Tree()
        {
            root = null;
        }
    }

    class ViewerSample
    {
        public static void DFS(string vertex, string searchVal, Microsoft.Msagl.Drawing.Graph graph)
        {
            //graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            MessageBox.Show(graph.FindNode(vertex).Attr.Id);
            if (graph.FindNode(vertex).Attr.Id == searchVal)
            {
                graph.FindNode(vertex).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
            }
        }
        public static void findDir(string docPath, Microsoft.Msagl.Drawing.Graph graph, Tree tree)
        {
            //Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            // Enumerasi docPath
            string root = docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            tree.root = new Node(root);

            List<string> dirs = new List<string>(Directory.EnumerateDirectories(docPath));
            List<string> files = new List<string>(Directory.EnumerateFiles(docPath));

            string parentFolderName = "";

            foreach (var dir in dirs)
            {
                //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                parentFolderName = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                // Console.WriteLine($"dir: {dir}");
                graph.AddEdge(root, parentFolderName);
                tree.root.child = new Node(parentFolderName);
                findDir(dir, graph, tree);
                
            }

            foreach (var file in files)
            {
                //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                parentFolderName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                // Console.WriteLine($"dir: {dir}");
                graph.AddEdge(root, parentFolderName);
                tree.root.child = new Node(parentFolderName);

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

            Tree tree = new Tree();
            tree.root = new Node(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1));

            findDir(docPath, graph, tree);
            DFS(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), GlobalVar.searchVal, graph);

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