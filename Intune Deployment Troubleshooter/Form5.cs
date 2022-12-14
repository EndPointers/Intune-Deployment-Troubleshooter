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

        int matchesFound = 0;
        int findDirection = 1;
        int CurrentIndex = 0;
        static List<int> _WordMatchIndexes = new List<int>();

        public Form5(Form parentForm)
        {
            mainForm = parentForm as Form1;
            InitializeComponent();
        }

        private void StoreMatchIndexes(DataGridView dgv, string val)
        {
            var WordMatchIndexes = _WordMatchIndexes;
            for (int x = 0; x < dgv.Rows.Count; x++)
            {
                int wordstartIndex = dgv.Rows[x].Cells[4].Value.ToString().IndexOf(val, StringComparison.OrdinalIgnoreCase);
                if (wordstartIndex != -1)
                {
                    WordMatchIndexes.Add(x);
                    matchesFound++;
                }
                else
                {
                    break;
                }
            }
        }

        private void FindNext(DataGridView dgv)
        {
            var WordMatchIndexes = _WordMatchIndexes;
            if (CurrentIndex <= WordMatchIndexes.Count)
            {
                CurrentIndex++;
                dgv.ClearSelection();
                dgv.Rows[WordMatchIndexes[CurrentIndex]].Selected = true;
                dgv.CurrentCell = dgv.Rows[WordMatchIndexes[CurrentIndex]].Cells[0];
            }
        }

        private void FindPrevious(DataGridView dgv)
        {
            var WordMatchIndexes = _WordMatchIndexes;
            if (CurrentIndex >= 1)
            {
                CurrentIndex--;
                dgv.ClearSelection();
                dgv.Rows[WordMatchIndexes[CurrentIndex]].Selected = true;
                dgv.CurrentCell = dgv.Rows[WordMatchIndexes[CurrentIndex]].Cells[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StoreMatchIndexes(this.mainForm.dataGridView1, textBox1.Text);
            if (_WordMatchIndexes.Count > 0)
            {
                switch (findDirection)
                {
                    case 0: //up
                        FindPrevious(this.mainForm.dataGridView1);
                        break;
                    case 1: //down
                        FindNext(this.mainForm.dataGridView1);
                        break;
                }
            }
            else
            {
                MessageBox.Show("The search term was not found.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            findDirection = 0; //up
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            findDirection = 1; //down
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            matchesFound = 0;
            _WordMatchIndexes.Clear();
            findDirection = 1;
            CurrentIndex = 0;
        }
    }
}
