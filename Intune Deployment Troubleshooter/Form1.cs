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

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form1 : Form
    {
        public static string hostname = "";
        public static DataTable dt = new DataTable();
        public static BindingSource bs = new BindingSource();

        private static string currentLogViewed = "";

        public Form1()
        {
            InitializeComponent();
            textBox1.KeyUp += textBox1_KeyUp;
            ResetTreeView();
            dt.Columns.Add("Status", typeof(Image));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Thread", typeof(string));
            dt.Columns.Add("Component", typeof(string));
            dt.Columns.Add("Entry", typeof(string));
            dt.Columns.Add("Type", typeof(string));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResetTreeView()
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("root", "MDM Diagnostics");
            treeView1.Nodes["root"].Nodes.Add("mdm", "Intune Logs");
            treeView1.Nodes["root"].Nodes.Add("evt", "Event Viewer Logs");
            treeView1.EndUpdate();
        }

        private void MakeConnection(string host)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                try
                {
                    ResetTreeView();
                    dt.Rows.Clear();
                    currentLogViewed = "";

                    syncConnectedDeviceToolStripMenuItem.Enabled = false;
                    getConnectedDeviceInfoToolStripMenuItem.Enabled = false;
                    watcherOffToolStripMenuItem.Enabled = false;
                    watcherOnToolStripMenuItem.Enabled = false;

                    toolStripStatusLabel1.Text = "Connecting to " + host.Split(".")[0] + " ...";

                    DirectoryInfo d = new DirectoryInfo(@"\\" + host + "\\C$\\ProgramData\\Microsoft\\IntuneManagementExtension\\Logs");

                    FileInfo[] Files = d.GetFiles("*.log");

                    foreach (FileInfo file in Files)
                    {
                        treeView1.Nodes["root"].Nodes["mdm"].Nodes.Add(file.Name);
                    }

                    string[] EventViewerLogs = {
                        "Application.evtx",
                        "microsoft-windows-aad.*operational.*",
                        "microsoft-windows-appxdeploymentserver.*operational.*",
                        "microsoft-windows-assignedaccess.*admin.*",
                        "microsoft-windows-assignedaccess.*operational.*",
                        "microsoft-windows-assignedaccessbroker.*admin.*",
                        "microsoft-windows-assignedaccessbroker.*operational.*",
                        "microsoft-windows-crypto-ncrypt.*operational.*",
                        "microsoft-windows-devicemanagement-enterprise-diagnostics-provider.*admin.*",
                        "microsoft-windows-devicemanagement-enterprise-diagnostics-provider.*autopilot.*",
                        "microsoft-windows-devicemanagement-enterprise-diagnostics-provider.*operational.*",
                        "microsoft-windows-moderndeployment-diagnostics-provider.*autopilot.*",
                        "microsoft-windows-moderndeployment-diagnostics-provider.*diagnostics.*",
                        "microsoft-windows-moderndeployment-diagnostics-provider.*managementservice.*",
                        "microsoft-windows-provisioning-diagnostics-provider.*admin.*",
                        "microsoft-windows-shell-core.*operational.*",
                        "microsoft-windows-user device registration.*admin.*",
                        "Security.evtx",
                        "Setup.evtx",
                        "System.evtx"
                    };

                    foreach (string eventLogs in EventViewerLogs)
                    {
                        DirectoryInfo d2 = new DirectoryInfo(@"\\" + host + "\\C$\\Windows\\System32\\winevt\\Logs");

                        FileInfo[] Files2 = d2.GetFiles("*.evtx");

                        foreach (FileInfo file2 in Files2)
                        {
                            if (Regex.IsMatch(file2.Name, eventLogs, RegexOptions.IgnoreCase))
                            {
                                treeView1.Nodes["root"].Nodes["evt"].Nodes.Add(file2.Name);
                            }
                        }
                    }

                    treeView1.EndUpdate();
                    treeView1.ExpandAll();
                    toolStripStatusLabel1.Text = "Connected";
                    syncConnectedDeviceToolStripMenuItem.Enabled = true;
                    getConnectedDeviceInfoToolStripMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    toolStripStatusLabel1.Text = "Not Connected";
                }
            }
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
        
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm3 = new Form3();
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
            try
            {
                contents = File.ReadAllText("\\\\" + textBox1.Text + "\\C$\\ProgramData\\Microsoft\\IntuneManagementExtension\\Logs\\" + log);
                findToolStripMenuItem.Enabled = true;
                BuildMDMLogViewer(contents);
                watcherOffToolStripMenuItem.Enabled = true;
                watcherOnToolStripMenuItem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text != "MDM Diagnostics" && e.Node.Text != "Intune Logs" && e.Node.Text != "Event Viewer Logs")
            {
                if (e.Node.Parent.Text == "Intune Logs")
                {
                    currentLogViewed = e.Node.Text;
                    RefreshLogViewer(currentLogViewed);
                }
                if (e.Node.Parent.Text == "Event Viewer Logs")
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/K " + Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\system32\\eventvwr.exe " + textBox1.Text + " /l:C:\\Windows\\System32\\winevt\\Logs\\" + e.Node.Text;
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
                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bs.RemoveFilter();
        }

        private void watcherOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(watcherOnToolStripMenuItem.Checked) 
            {
                watcherOffToolStripMenuItem.Checked = true;
                watcherOnToolStripMenuItem.Checked = false;
            }
            toolStripStatusLabel2.Image = null;
            timer1.Enabled = false;
        }

        private void watcherOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (watcherOffToolStripMenuItem.Checked)
            {
                watcherOnToolStripMenuItem.Checked = true;
                watcherOffToolStripMenuItem.Checked = false;
            }
            toolStripStatusLabel2.Image = imageList1.Images[3];
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshLogViewer(currentLogViewed);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

    }
}