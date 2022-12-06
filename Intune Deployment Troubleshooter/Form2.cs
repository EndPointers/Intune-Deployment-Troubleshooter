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
using System.Text.RegularExpressions;
using Microsoft.Win32;
using static System.Net.WebRequestMethods;
using System.Diagnostics;

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Shown += new System.EventHandler(this.Form2_Shown);
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

        private void Form2_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            RunPowershellScript(Directory.GetCurrentDirectory() + "\\scripts\\Get-DeviceInfo.ps1", "hostname", Form1.hostname.Split(".")[0]);
            toolStripStatusLabel1.Text = "Done";
            openInIntuneToolStripMenuItem.Enabled = true;
        }

        private void OpenUrl(string url)
        {
            string browser = string.Empty;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command");
                if (key != null)
                {
                    browser = key.GetValue(null).ToString().ToLower().Trim(new[] { '"' });
                }
                if (!browser.EndsWith("exe"))
                {
                    browser = browser.Substring(0, browser.LastIndexOf(".exe", StringComparison.InvariantCultureIgnoreCase) + 4);
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
            Process proc = Process.Start(browser, url);
        }

        private void openInIntuneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String txtData = textBox1.Text.Replace("\r", "").Replace("\n", "");
            String deviceID = Regex.Replace(txtData, ".*Device ID: (.*?)", "$1", RegexOptions.IgnoreCase);
            String url= "https://endpoint.microsoft.com/#view/Microsoft_Intune_Devices/DeviceSettingsMenuBlade/~/overview/mdmDeviceId/" + deviceID;
            OpenUrl(url);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
