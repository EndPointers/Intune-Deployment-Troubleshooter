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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
            this.Shown += new System.EventHandler(this.Form6_Shown);
        }

        private void BuildDataGrid(string contents)
        {
            string[] data = contents.Split(":");
            if(data.Count() > 1)
            {
                try
                {
                    dataGridView1.Rows.Add(data[0], data[1]);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
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
                    BuildDataGrid(output.ToString());
                }
            }
            if (ps.Streams.Error.Count > 0)
            {
                MessageBox.Show(ps.Streams.Error[0].ToString());
            }
            rs.Close();
        }

        private void Form6_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            RunPowershellScript(Directory.GetCurrentDirectory() + "\\scripts\\Get-InstalledAppsPerDevice.ps1", "hostname", Form1.hostname.Split(".")[0]);
            toolStripStatusLabel1.Text = "Done";
        }

    }
}
