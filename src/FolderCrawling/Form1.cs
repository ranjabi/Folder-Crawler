namespace FolderCrawling

{
    public partial class Form1 : Form
    {
        static bool isFolderChosen = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                label4.Text = dialog.SelectedPath;
                isFolderChosen = true;
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            if (isFolderChosen)
            {
                string searchingPath = label4.Text + "\\...\\" + textBox1.Text;
                label8.Text = searchingPath;
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
}