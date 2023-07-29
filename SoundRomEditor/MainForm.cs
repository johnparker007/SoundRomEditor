using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundRomEditor
{
    public partial class MainForm : Form
    {
        // TOIMPROVE - not sure if MainForm should own the Project really
        public Project Project
        {
            get;
            private set;
        }

        public MainForm()
        {
            InitializeComponent();

            Project = new Project();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Project.SaveWav();
        }
    }
}
