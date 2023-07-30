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
    public partial class ViewMainForm : Form
    {
        public ViewMainForm()
        {
            InitializeComponent();
        }

        private void OnButtonPlayAllClick(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.ViewModelMainForm.PlayAllSamples();
        }

        private void OnButtonSaveAllWavsClick(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.ViewModelMainForm.SaveAllWavs();
        }

        private void OnButtonLoadRomsClick(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.ViewModelMainForm.LoadRoms();
        }
    }
}
