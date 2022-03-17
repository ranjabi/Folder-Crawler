using System;
using System.IO;
using System.Collections;
using Microsoft.Msagl;
using Microsoft.Msagl.Splines;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Edge = Microsoft.Msagl.Drawing.Edge;
using MsaglDraw = Microsoft.Msagl.Drawing;
namespace FolderCrawling

{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void searchBtn_Click(object sender, EventArgs e)
        // search button
        {
            if (GlobalVar.isFolderChoosen)
            {
                string searchingPath = label4.Text + "\\...\\" + textBox1.Text;
                label8.Text = searchingPath;
                GlobalVar.searchVal = textBox1.Text;
                GlobalVar.edges.Clear();
                GlobalVar.visited.Clear();
                Microsoft.Msagl.GraphViewerGdi.GViewer graphViewer = GraphViewer.Launch();
                panel1.Controls.Clear();
                panel1.SuspendLayout();
                panel1.Controls.Add(graphViewer);
                panel1.ResumeLayout();
                //show the form 
                panel1.Show();
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

        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    // Determine which link was clicked within the LinkLabel.
        //    this.linkLabel1.Links[linkLabel1.Links.IndexOf(e.Link)].Visited = true;

        //    // Display the appropriate link based on the value of the 
        //    // LinkData property of the Link object.
        //    string target = e.Link.LinkData as string;

        //    // If the value looks like a URL, navigate to it.
        //    // Otherwise, display it in a message box.
        //    if (null != target && target.StartsWith("www"))
        //    {
        //        System.Diagnostics.Process.Start(target);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Item clicked: " + target);
        //    }
        //}
        
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
            public static Node DFS(List<string> foundPath, bool allOccurence, string searchVal, Dictionary<string, bool> visited, Node tree, Microsoft.Msagl.Drawing.Graph graph)
            // perform BFS
            {
                // using regex to find searchVal followed by whitespace at the end of the word
                searchVal = @"^" + searchVal + @"\s*\b";


                graph.FindNode(tree.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                // path biru ketimpa lagi sama path merah
                //if (!(GlobalVar.edges[tree.prevPath.path+tree.path].Attr.Color) && (tree.prevPath != null))
                //{
                //    colorPath(tree.prevPath, tree, "red");
                //}
                visited[tree.path] = true;

                if (Regex.IsMatch(tree.Name, searchVal))
                {
                    graph.FindNode(tree.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    colorPath(tree.prevPath, tree, "blue");
                    foundPath.Add(tree.path);
                    //MessageBox.Show(tree.path + "1");
                    if (!allOccurence)
                    {
                        return tree;
                    }
                }

                foreach (Node temp in tree.Children)
                {
                    if (!visited[temp.path])
                    {
                        Node find = DFS(foundPath, allOccurence, searchVal, visited, temp, graph); //
                        if (Regex.IsMatch(find.Name, searchVal))
                        {
                            graph.FindNode(find.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                            MessageBox.Show(tree.path + "\n" + temp.path + "\n" + find.path);
                            //GlobalVar.edges[tree.path + temp.path].Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                            colorPath(tree, temp, "blue");
                            //foundPath.Append(tree.path);
                            //MessageBox.Show(tree.path + "3");
                            if (!allOccurence)
                            {
                                //break;
                                return find;
                            }
                        }
                    }
                }
                return tree;

            }

            public static void BFS(string docPath, Node startNode, string searchVal, Microsoft.Msagl.Drawing.Graph graph, bool allOccurence)
            // perform DFS
            {
                bool found = false;
                searchVal = @"^" + searchVal + @"\s*\b";
                Queue<Node> queue = new Queue<Node>();
                GlobalVar.visited[startNode.path] = true;
                if (Regex.IsMatch(startNode.Name, searchVal))
                {
                    graph.FindNode(startNode.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                    found = true;
                } else
                {
                    graph.FindNode(startNode.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                }
                queue.Enqueue(startNode);



                    while ((queue.Count > 0) && !found)
                    {
                        Node curNode = queue.Dequeue();
                    //foreach (Node node in curNode.Children)
                    //{
                    //    MessageBox.Show(node.path);
                    //}
                    foreach (Node temp in curNode.Children)
                        {
                            if (!GlobalVar.visited[temp.path])
                            {
                                GlobalVar.visited[temp.path] = true;
                            if (Regex.IsMatch(temp.Name, searchVal))
                            {
                                colorPath(curNode, temp, "blue");
                                graph.FindNode(temp.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                                if (!allOccurence)
                                {
                                    found = true;
                                }
                            }
                            else
                            {
                                graph.FindNode(temp.Name).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                            }
                                queue.Enqueue(temp);
                            }
                        }
                    }
            }

            public static void colorPath(Node source, Node target, string color)
            // fill the edge color between source and target node
            {
                if (target.prevPath != null)
                {
                    //MessageBox.Show(source.path + "," + target.path);
                    if (color == "red")
                    {
                        GlobalVar.edges[source.path + target.path].Attr.Color = MsaglDraw.Color.Red;
                    } else if (color == "blue")
                    {
                        GlobalVar.edges[source.path + target.path].Attr.Color = MsaglDraw.Color.Blue;
                    }
                    colorPath(source.prevPath, target.prevPath, color);
                }
            }

            public static void findDir(string docPath, Microsoft.Msagl.Drawing.Graph graph, Node tree)
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
                        //MessageBox.Show(root + "," + parentFolderName);
                        GlobalVar.edges.Add(docPath+dir, tempEdge);
                        //graph.RemoveEdge(temps);
                        //graph.RemoveEdge(graph.EdgeById)
                        findDir(temp.path, graph, temp);

                    }
                }

                foreach (var file in files)
                {
                    //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                    parentFolderName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    // Console.WriteLine($"dir: {dir}");
                    Node temp = new Node(parentFolderName, tree, file, new List<Node>());

                    tree.Children.Add(temp);
                    Edge tempEdge = graph.AddEdge(root, parentFolderName);
                    GlobalVar.edges.Add(docPath+file, tempEdge);

                }
                // return parentFolderName;
            }
            public static Microsoft.Msagl.GraphViewerGdi.GViewer Launch()
            {


                //create a form 
                //System.Windows.Forms.Form form = new System.Windows.Forms.Form();
                //create a viewer object 
                Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
                //create a graph object 
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");


                //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string docPath = GlobalVar.selectedPath;

                Node tree = new Node(docPath.Substring(docPath.LastIndexOf(Path.DirectorySeparatorChar) + 1), null, docPath, new List<Node>());

                findDir(tree.path, graph, tree);

                
                initVisited(tree, GlobalVar.visited);
                ////////////////////////////
                if (GlobalVar.method == "DFS")
                {
                    DFS(GlobalVar.foundPath, GlobalVar.allOccurence, GlobalVar.searchVal, GlobalVar.visited, tree, graph);
                } else if (GlobalVar.method == "BFS")
                {

                    BFS(GlobalVar.selectedPath, tree, GlobalVar.searchVal, graph, GlobalVar.allOccurence);
                }

                //GlobalVar.edges["srcconfig"].Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                //bind the graph to the viewer 
                viewer.Graph = graph;
                //associate the viewer with the form 
                viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                return viewer;


            }
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
        public static List<string> foundPath = new List<string>();
        public static Dictionary<string, Edge> edges = new Dictionary<string, Edge>();
        public static Dictionary<string, bool> visited = new Dictionary<string, bool>();
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

    class ViewerSample
    {
}