using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System;
using System.ComponentModel;
using Microsoft.VisualBasic;
using Microsoft.PowerShell.Commands;
using System.Windows.Forms;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form1 : Form
    {
        public static string hostname = "";
        public static DataTable dt = new DataTable();
        public static BindingSource bs = new BindingSource();
        private static string currentLogViewed = "";
        public static string UserLogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EndPointers\\Logs\\";

        public Form1()
        {
            InitializeComponent();
            textBox1.KeyUp += textBox1_KeyUp;
            System.IO.Directory.CreateDirectory(UserLogPath);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private TreeNode ResetTVParentNode()
        {
            TreeNode ParentNode = treeView1.Nodes["root"].Nodes["evt"];
            return ParentNode;
        }

        private void ResetUIDefaults()
        {
            treeView1.Nodes.Clear();
            dt.Rows.Clear();
            dt.Columns.Clear();
            ClearLogs();
            currentLogViewed = "";
            findToolStripMenuItem.Enabled = false;
            createToolStripMenuItem.Enabled = false;
            bs.RemoveFilter();
            removeToolStripMenuItem.Enabled = false;
            syncConnectedDeviceToolStripMenuItem.Enabled = false;
            getConnectedDeviceInfoToolStripMenuItem.Enabled = false;
            getConnectedDeviceDiscoveredAppsToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = "Not Connected";
        }

        private void MakeConnection(string host)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                try
                {
                    toolStripStatusLabel1.Text = "Connecting to " + host.Split(".")[0] + " ...";

                    DirectoryInfo d = new DirectoryInfo(@"\\" + host + "\\C$\\ProgramData\\Microsoft\\IntuneManagementExtension\\Logs");

                    FileInfo[] Files = d.GetFiles("*.log");

                    ResetUIDefaults();
                    treeView1.Nodes.Add("root", "MDM Diagnostics");
                    treeView1.Nodes["root"].Nodes.Add("mdm", "Intune Logs");
                    treeView1.Nodes["root"].Nodes.Add("evt", "Event Viewer Logs");
                    treeView1.Nodes["root"].Nodes["evt"].Nodes.Add("Microsoft", "Microsoft");
                    treeView1.Nodes["root"].Nodes["evt"].Nodes["Microsoft"].Nodes.Add("Windows", "Windows");

                    foreach (FileInfo file in Files)
                    {
                        File.Copy(file.FullName, UserLogPath + "\\" + file.Name, true);
                        treeView1.Nodes["root"].Nodes["mdm"].Nodes.Add(file.Name,file.Name,1,1);
                    }

                    string[] EventViewerLogs = {
                        @"Application.evtx",
                        @"microsoft-windows-aad.*operational.evtx",
                        @"microsoft-windows-appxdeploymentserver.*operational.evtx",
                        @"microsoft-windows-assignedaccess.*admin.evtx",
                        @"microsoft-windows-assignedaccess.*operational.evtx",
                        @"microsoft-windows-crypto-ncrypt.*operational.evtx",
                        @"microsoft-windows-devicemanagement-enterprise-diagnostics-provider.*admin.evtx",
                        @"microsoft-windows-devicemanagement-enterprise-diagnostics-provider.*autopilot.evtx",
                        @"microsoft-windows-devicemanagement-enterprise-diagnostics-provider.*operational.evtx",
                        @"microsoft-windows-moderndeployment-diagnostics-provider.*autopilot.evtx",
                        @"microsoft-windows-moderndeployment-diagnostics-provider.*diagnostics.evtx",
                        @"microsoft-windows-moderndeployment-diagnostics-provider.*managementservice.evtx",
                        @"microsoft-windows-provisioning-diagnostics-provider.*admin.evtx",
                        @"microsoft-windows-shell-core.*operational.evtx",
                        @"microsoft-windows-user device registration.*admin.evtx",
                        @"Security.evtx",
                        @"Setup.evtx",
                        @"System.evtx"
                    };

                    TreeNode ParentNode = ResetTVParentNode();

                    foreach (string eventLogs in EventViewerLogs)
                    {
                        DirectoryInfo d2 = new DirectoryInfo(@"\\" + host + "\\C$\\Windows\\System32\\winevt\\Logs");

                        FileInfo[] Files2 = d2.GetFiles("*.evtx");

                        foreach (FileInfo file2 in Files2)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(file2.Name, eventLogs, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                string fileName = file2.Name;
                                string[] vPaths = fileName.Split("%4");
                                if (vPaths.Count() > 1)
                                {
                                    string[] vFolders = vPaths[0].Split("-");
                                    for (int x = 0; x < vFolders.Count(); x++)
                                    {
                                        if (isNodeChildNode(ParentNode, vFolders[x]) == false)
                                        {
                                            ParentNode.Nodes.Add(vFolders[x], vFolders[x], 0, 0);
                                            ParentNode = ParentNode.Nodes[vFolders[x]];
                                        }
                                        else
                                        {
                                            ParentNode = ParentNode.Nodes[vFolders[x]];
                                        }
                                    }

                                    ParentNode.Nodes.Add(vPaths[1], vPaths[1].Replace(".evtx", ""), 1, 1);
                                    ParentNode = ResetTVParentNode();
                                }
                                else
                                {
                                    treeView1.Nodes["root"].Nodes["evt"].Nodes.Add(file2.Name, file2.Name.Replace(".evtx", ""), 1, 1);
                                }
                            }
                        }
                    }

                    treeView1.EndUpdate();
                    treeView1.Nodes["root"].Expand();
                    treeView1.Nodes["root"].Nodes["mdm"].Expand();
                    treeView1.Nodes["root"].Nodes["evt"].Expand();
                    treeView1.Nodes["root"].Nodes["evt"].Nodes["Microsoft"].Expand();
                    treeView1.Nodes["root"].Nodes["evt"].Nodes["Microsoft"].Nodes["Windows"].Expand();
                    toolStripStatusLabel1.Text = "Connected";
                    syncConnectedDeviceToolStripMenuItem.Enabled = true;
                    getConnectedDeviceInfoToolStripMenuItem.Enabled = true;
                    getConnectedDeviceDiscoveredAppsToolStripMenuItem.Enabled = true;
                    treeView1.Nodes[0].EnsureVisible();
                }
                catch (Exception ex)
                {
                    ResetUIDefaults();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private bool isNodeChildNode(TreeNode t, string val)
        {
            bool result = false;

            foreach (TreeNode p in t.Nodes)
            {
                if (p.Name == val)
                {
                    result = true; break;
                }
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakeConnection(textBox1.Text);
        }

        private Image GetStatus(string entry)
        {
            Image result = imageList1.Images[1];
            if(Regex.Match(entry, @"(failed)|(error \d+)",RegexOptions.IgnoreCase).Success)
            {
                result = imageList1.Images[0];
            }
            else if (Regex.Match(entry, @"(unable)|(could not)|(falling back)", RegexOptions.IgnoreCase).Success)
            {
                result = imageList1.Images[2];
            }
            return result;
        }

        private void BuildMDMLogViewer(string contents)
        {
            string logData = contents.Replace("\n", string.Empty).Replace("\r",string.Empty);
            var pattern = "<\\!\\[LOG\\[(.*?)\\]LOG\\]\\!><time=\"(.*?)\" date=\"(.*?)\" component=\"(.*?)\" context=\"(.*?)\" type=\"(.*?)\" thread=\"(.*?)\" file=\"(.*?)\">";
            string replace = "$2 $3~$7~$4~$1~$5~$6~$8`";
            string outData = Regex.Replace(logData, pattern, replace, RegexOptions.IgnoreCase);
            dt.Columns.Add("Status", typeof(Image));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Thread", typeof(string));
            dt.Columns.Add("Component", typeof(string));
            dt.Columns.Add("Entry", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            foreach (string data in outData.Split("`"))
            {
                string[] record = data.Split("~");
                if (record.Length > 0)
                {
                    try
                    {
                        dt.Rows.Add(GetStatus(record[3]), record[0], record[1], record[2], record[3], record[5]);
                    } catch (Exception) 
                    { //DoNothing
                    }
                }
            }
            bs.DataSource = dt;
            dataGridView1.DataSource = bs;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Intune Deployment Troubleshooter\nVersion 1.1.15\nDeveloped by James Everett", "About");
        }

        private void syncConnectedDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hostname = textBox1.Text;
            Form4 frm4 = new Form4();
            frm4.ShowDialog();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm3 = new Form3(this);
            frm3.ShowDialog();
        }

        private void getConnectedDeviceInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hostname = textBox1.Text;
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }

        private void textBox1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                MakeConnection(textBox1.Text);
            }
        }

        private void RefreshLogViewer(string log)
        {
            String contents = "";
            dt.Rows.Clear();
            dt.Columns.Clear();
            try
            {
                contents = File.ReadAllText(UserLogPath + log);
                createToolStripMenuItem.Enabled = true;
                findToolStripMenuItem.Enabled = true;
                BuildMDMLogViewer(contents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string DecodeFileName(TreeNode node)
        {
            string result = "";
            List<string> CleanPath = new List<string>();
            string[] nodes = node.FullPath.Split("\\");
            if (nodes.Count() > 3)
            {
                for (int x = nodes.Count() - 1; x >= 0; x--)
                {
                    if (nodes[x] != "MDM Diagnostics" && nodes[x] != "Intune Logs" && nodes[x] != "Event Viewer Logs")
                    {
                        if (x == nodes.Count() - 1)
                        {
                            CleanPath.Add("%4" + nodes[x]);
                        }
                        else
                        {
                            CleanPath.Add("-" + nodes[x]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                CleanPath.Reverse();
                foreach (string CP in CleanPath)
                {
                    result += CP;
                }
                result = result.Substring(1, result.Length - 1);
            } else
            {
                result = nodes[2];
            }
            return result + ".evtx";
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text != "MDM Diagnostics" && e.Node.Text != "Intune Logs" && e.Node.Text != "Event Viewer Logs")
            {
                if (e.Node.Parent.Text == "Intune Logs")
                {
                    currentLogViewed = e.Node.Text;
                    RefreshLogViewer(currentLogViewed);
                } else 
                {
                    string evtFileName = DecodeFileName(e.Node);

                    if (evtFileName.IndexOf(".evtx") >= 0)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/K " + Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\system32\\eventvwr.exe " + textBox1.Text + " /l:C:\\Windows\\System32\\winevt\\Logs\\" + evtFileName;
                        startInfo.RedirectStandardOutput = true;
                        startInfo.RedirectStandardError = true;
                        startInfo.UseShellExecute = false;
                        startInfo.CreateNoWindow = true;

                        Process processTemp = new Process();
                        processTemp.StartInfo = startInfo;
                        processTemp.EnableRaisingEvents = true;
                        try
                        {
                            processTemp.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void ClearLogs()
        {
            DirectoryInfo di = new DirectoryInfo(UserLogPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bs.RemoveFilter();
            removeToolStripMenuItem.Enabled = false;
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 frm5 = new Form5(this);
            frm5.ShowDialog();
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            ClearLogs();
        }

        private void getConnectedDeviceDiscoveredAppsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hostname = textBox1.Text;
            Form6 frm6 = new Form6();
            frm6.Show();
        }
    }
}