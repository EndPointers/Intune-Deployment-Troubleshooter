using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            this.Shown += new System.EventHandler(this.Form4_Shown);
        }

        private void RunPowershellScript(String cmd, String arg, String val)
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();

            Runspace rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;
            ps.AddCommand("Set-ExecutionPolicy").AddParameter("ExecutionPolicy", "Bypass");
            ps.AddCommand(cmd).AddParameter("hostname", val);
            var results = ps.Invoke();
            foreach (PSObject output in results)
            {
                if (output != null)
                {
                    textBox1.Text += output.ToString() + "\r\n";
                }
            }
            if (ps.Streams.Error.Count > 0)
            {
                textBox1.Text += ps.Streams.Error[0].ToString();
            }
            rs.Close();
        }

        private void Form4_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            RunPowershellScript(Directory.GetCurrentDirectory() + "\\scripts\\Sync-DevicePerHostname.ps1", "hostname", Form1.hostname.Split(".")[0]);
            toolStripStatusLabel1.Text = "Done";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
