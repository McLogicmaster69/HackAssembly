using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assembler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void assembleBtn_Click(object sender, EventArgs e)
        {
            string[] input = inputTxt.Text.Split('\n');
            string[] output = MainAssembler.Assemble(input);
            string combined = "";
            foreach (string s in output)
            {
                combined += s;
            }
            outputTxt.Text = combined;
        }
    }
}
