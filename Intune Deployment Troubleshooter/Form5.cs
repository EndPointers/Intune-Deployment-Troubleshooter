using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intune_Deployment_Troubleshooter
{
    public partial class Form5 : Form
    {
        private Form1 mainForm = null;
        private static int up = -1;
        private static int down = 1;
        int findDirection = down;
        int CurrentRow = 1;
        bool found = false;


        public Form5(Form parentForm)
        {
            mainForm = parentForm as Form1;
            InitializeComponent();
        }

        private void FindMatch(DataGridView dgv, string val, int startIndex)
        {
            switch(findDirection)
            {
                case -1:
                    FindPrevious(dgv, val, startIndex);
                    break;
                case 1:
                    FindNext(dgv, val, startIndex);
                    break;
            }
        }

        private void FindNext(DataGridView dgv, string val, int startIndex)
        {
            int wordstartIndex = -1;
            if (startIndex <= dgv.Rows.Count)
            {
                for (int x = startIndex; x < dgv.Rows.Count; x++)
                {
                    if (dgv.Rows[x].Cells[4].Value.ToString() != string.Empty)
                    {
                        string source = dgv.Rows[x].Cells[4].Value.ToString();
                        if (checkBox1.Checked)
                        {
                            wordstartIndex = source.IndexOf(val);
                        }
                        else
                        {
                            wordstartIndex = source.IndexOf(val, StringComparison.OrdinalIgnoreCase);
                        }
                        if (wordstartIndex != -1)
                        {
                            CurrentRow = x;
                            dgv.ClearSelection();
                            dgv.Rows[CurrentRow].Selected = true;
                            dgv.CurrentCell = dgv.Rows[CurrentRow].Cells[0];
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    MessageBox.Show("The search term was not found.");
                }
            }
        }

        private void FindPrevious(DataGridView dgv, string val, int startIndex)
        {
            int wordstartIndex = -1;
            if (startIndex >= 0)
            {
                for (int x = startIndex; x < dgv.Rows.Count; x--)
                {
                    if (dgv.Rows[x].Cells[4].Value.ToString() != string.Empty)
                    {
                        string source = dgv.Rows[x].Cells[4].Value.ToString();
                        if (checkBox1.Checked)
                        {
                            wordstartIndex = source.IndexOf(val);
                        }
                        else
                        {
                            wordstartIndex = source.IndexOf(val, StringComparison.OrdinalIgnoreCase);
                        }
                        if (wordstartIndex != -1)
                        {
                            CurrentRow = x;
                            dgv.ClearSelection();
                            dgv.Rows[CurrentRow].Selected = true;
                            dgv.CurrentCell = dgv.Rows[CurrentRow].Cells[0];
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    MessageBox.Show("The search term was not found.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int nowRow = CurrentRow + findDirection;
            FindMatch(this.mainForm.dataGridView1, textBox1.Text, nowRow);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            findDirection = up;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            findDirection = down;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CurrentRow = 1;
            found = false;
        }
    }
}
