using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            comboBox1.Items.Add("Entry");
            comboBox1.Items.Add("Component");
            comboBox1.Items.Add("Thread");
        }

        private void FilterDataGridView(bool textChecked, string columnName, string findVal, bool timeChecked, string timeVal)
        {
            if (textChecked == true && timeChecked == false)
            {
                Form1.bs.Filter = string.Format(columnName + " LIKE '%{0}%'", findVal);
            }
            if(timeChecked == true && textChecked == false)
            {
                Form1.bs.Filter = string.Format("Time LIKE '%{0}%'", timeVal);
            }
            if(timeChecked == true && textChecked == true)
            {
                Form1.bs.Filter = string.Format(columnName + " LIKE '%{0}%' AND Time LIKE '%{1}%'", findVal,timeVal);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FilterDataGridView(checkBox2.Checked, comboBox1.GetItemText(comboBox1.SelectedItem), textBox1.Text, checkBox1.Checked, textBox2.Text);
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox2.Enabled = true;
                button1.Enabled = true;
            } else
            {
                textBox2.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
                button1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                button1.Enabled = false;
            }
        }
    }
}
