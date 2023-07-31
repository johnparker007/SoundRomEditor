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

        private void ViewMainForm_Load(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.Project.OnLoadRomsCompleted += OnLoadRomsCompleted;
        }

        private void OnLoadRomsCompleted()
        {
            // TODO all this will probably want to go into some kind of Refresh functions
            SamplesDataGridView.DataSource = SoundRomEditor.Instance.Project.Samples;

            SamplesDataGridView.Columns["Duration"].DefaultCellStyle.Format = "N2";

            List<Sample> samples = SoundRomEditor.Instance.Project.Samples;
            for (int sampleIndex = 0; sampleIndex < samples.Count; ++sampleIndex)
            {
                if (samples[sampleIndex].SampleCount == 0)
                {
                    CurrencyManager currencyManager = (CurrencyManager)BindingContext[SamplesDataGridView.DataSource];
                    currencyManager.SuspendBinding();
                    SamplesDataGridView.Rows[sampleIndex].Visible = false;
                    currencyManager.ResumeBinding();
                }
            }
        }

        private void OnSamplesDataGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex == 0)
            {
                Console.WriteLine("TODO sort column");
                // TODO header sort
            }
            else
            {
                SoundRomEditor.Instance.Project.Samples[e.RowIndex].Play(false);
            }
        }

        private void SamplesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
        }
    }
}
