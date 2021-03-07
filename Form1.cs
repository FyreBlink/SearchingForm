using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchingForm
{
    public partial class Form1 : Form
    {
        List<string> foundFiles;
        int countFiles;
        static string workingDir = Environment.CurrentDirectory;
        static string projectDir = Directory.GetParent(workingDir).Parent.FullName;
        public Form1()
        {
            InitializeComponent();

            PathLabel.Text = "";
            FoundFilesLabel.Text = "";
            foundFiles = new();

            string filePath = projectDir + "\\PreviousSession.txt";
            try
            {
                StreamReader sr = new StreamReader(filePath);
                PathLabel.Text = sr.ReadLine();
                QueryTextBox1.Text = sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex) { }
        }

        //Создание стартового узла
        private void CreateStartNode(string path)
        {
            try
            {
                TreeNode startNode = new TreeNode { Text = path.TrimEnd('\\') };
                FillTreeNode(startNode, path);
                treeView1.Nodes.Add(startNode);
            }
            catch (Exception ex) { }
        }

        //Получение дочерних узлов для определенного узла
        private void FillTreeNode(TreeNode baseNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
                    baseNode.Nodes.Add(dirNode);
                }

                string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                foreach (string f in files)
                {
                    TreeNode fileNode = new();
                    fileNode.Text = f.Remove(0, f.LastIndexOf("\\") + 1);
                    baseNode.Nodes.Add(fileNode);
                }
            }
            catch (Exception ex) { }
        }

        //Рекурсивное получение файлов
        private void RecuGettingFile(string path, int selector)
        {
            try
            {
                switch (selector)
                {
                    case 1:
                        foreach (string f in Directory.GetFiles(path))
                        {
                            countFiles++;
                        }
                        break;
                    case 2:
                        Regex reg = new(QueryTextBox1.Text);
                        var files = Directory.GetFiles(path).Where(path => reg.IsMatch(path));
                        foreach (string f in files)
                        {
                            foundFiles.Add(f);
                        }
                        break;
                    default:
                        break;
                }

                foreach (string d in Directory.GetDirectories(path))
                {
                    RecuGettingFile(d, selector);
                }
            }
            catch (Exception ex) { }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            DirectoryInfo info = new(dirs[i]);
                            if ((info.Attributes & FileAttributes.Hidden) == 0)
                            {
                                int n = 0;
                                int j = 0;

                                do
                                {
                                    if ((foundFiles.Count == 0) & (QueryTextBox1.Text == ""))
                                    {
                                        n = 1;
                                        break;
                                    }
                                    else if (foundFiles[j].Length > dirs[i].Length)
                                    {
                                        if (dirs[i] == foundFiles[j][..dirs[i].Length])
                                        {
                                            n = 1;
                                            break;
                                        }
                                    }
                                    j++;
                                } while ((n == 0) & (j < foundFiles.Count)) ;

                                if (n == 1)
                                {
                                    TreeNode dirNode = new(new DirectoryInfo(dirs[i]).Name);
                                    FillTreeNode(dirNode, dirs[i]);
                                    e.Node.Nodes.Add(dirNode);
                                }
                            }
                        }
                    }

                    string[] files = Directory.GetFiles(e.Node.FullPath, "*", SearchOption.TopDirectoryOnly);
                    foreach (string f in files)
                    {
                        if ((foundFiles.Count == 0) & (QueryTextBox1.Text == ""))
                        {
                            TreeNode fileNode = new();
                            fileNode.Text = f.Remove(0, f.LastIndexOf("\\") + 1);
                            e.Node.Nodes.Add(fileNode);
                        }
                        else
                        {
                            for (int i = 0; i < foundFiles.Count; i++)
                            {
                                if (f == foundFiles[i])
                                {
                                    TreeNode fileNode = new();
                                    fileNode.Text = f.Remove(0, f.LastIndexOf("\\") + 1);
                                    e.Node.Nodes.Add(fileNode);
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex) { }
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            DirectoryInfo info = new(dirs[i]);
                            if ((info.Attributes & FileAttributes.Hidden) == 0)
                            {
                                int n = 0;
                                int j = 0;

                                do
                                {
                                    if ((foundFiles.Count == 0) & (QueryTextBox1.Text == ""))
                                    {
                                        n = 1;
                                        break;
                                    }
                                    else if (foundFiles[j].Length > dirs[i].Length)
                                    {
                                        if (dirs[i] == foundFiles[j][..dirs[i].Length])
                                        {
                                            n = 1;
                                            break;
                                        }
                                    }
                                    j++;
                                } while ((n == 0) & (j < foundFiles.Count));

                                if (n == 1)
                                {
                                    TreeNode dirNode = new(new DirectoryInfo(dirs[i]).Name);
                                    FillTreeNode(dirNode, dirs[i]);
                                    e.Node.Nodes.Add(dirNode);
                                }
                            }
                        }
                    }

                    string[] files = Directory.GetFiles(e.Node.FullPath, "*", SearchOption.TopDirectoryOnly);
                    foreach (string f in files)
                    {
                        if ((foundFiles.Count == 0) & (QueryTextBox1.Text == ""))
                        {
                            TreeNode fileNode = new();
                            fileNode.Text = f.Remove(0, f.LastIndexOf("\\") + 1);
                            e.Node.Nodes.Add(fileNode);
                        }
                        else
                        {
                            for (int i = 0; i < foundFiles.Count; i++)
                            {
                                if (f == foundFiles[i])
                                {
                                    TreeNode fileNode = new();
                                    fileNode.Text = f.Remove(0, f.LastIndexOf("\\") + 1);
                                    e.Node.Nodes.Add(fileNode);
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex) { }
        }

        //Событие при нажатии кнопки "Choose a Directory"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                countFiles = 0;
                FolderBrowserDialog FBD = new();
                FBD.ShowNewFolderButton = false;
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    PathLabel.Text = FBD.SelectedPath;
                }
                treeView1.Nodes.Clear();
                CreateStartNode(FBD.SelectedPath);
                RecuGettingFile(FBD.SelectedPath, 1);
                label4.Text = "Total Files: " + countFiles.ToString();

                if (QueryTextBox1.Text != "")
                {
                    button2_Click(button2, e);
                }
            }
            catch (Exception ex) { }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                foundFiles = new();
                TimeSpan span = new();
                DateTime startTime = DateTime.Now;
                RecuGettingFile(PathLabel.Text, 2);
                DateTime finishTime = DateTime.Now;
                span = finishTime.Subtract(startTime);

                FoundFilesLabel.Text = foundFiles.Count.ToString();
                label5.Text = "Time Elapsed: " + span.ToString("T");

                treeView1.Nodes.Clear();
                CreateStartNode(PathLabel.Text);
            }
            catch (Exception) { }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string filePath = projectDir + "\\PreviousSession.txt";
                StreamWriter sw = new(filePath, false, System.Text.Encoding.Default);
                sw.WriteLine(PathLabel.Text);
                sw.WriteLine(QueryTextBox1.Text);
                sw.Flush();
            }
            catch (Exception ex) { }
        }
    }
}
