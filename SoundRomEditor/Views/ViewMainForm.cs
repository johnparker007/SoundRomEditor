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
        public Sample SelectedSample
        {
            get
            {
                int selectedRowIndex = SamplesDataGridView.SelectedRows[0].Index;
                return SoundRomEditor.Instance.Project.Samples[selectedRowIndex];
            }
        }

        public ViewMainForm()
        {
            InitializeComponent();

            SoundRomEditor.Instance.ViewModelMainForm.SetViewMainForm(this);
        }

        private void OnViewMainFormLoad(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.Project.OnLoadRomsCompleted += OnLoadRomsCompleted;
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

        private void OnButtonSaveRomsClick(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.ViewModelMainForm.SaveRoms();
        }

        private void OnButtonLoadWavClick(object sender, EventArgs e)
        {
            SoundRomEditor.Instance.ViewModelMainForm.LoadWav();
        }

        private void OnLoadRomsCompleted()
        {
            // TODO all this will probably want to go into some kind of Refresh functions
            SamplesDataGridView.DataSource = SoundRomEditor.Instance.Project.Samples;

            SamplesDataGridView.Columns["OriginalSampleData"].Visible = false;
            SamplesDataGridView.Columns["OverrideSampleData"].Visible = false;

            SamplesDataGridView.Columns["Duration"].DefaultCellStyle.Format = "N2";

            List<Sample> samples = SoundRomEditor.Instance.Project.Samples;
            for (int sampleIndex = 0; sampleIndex < samples.Count; ++sampleIndex)
            {
                if (samples[sampleIndex].OriginalSampleData.SampleCount == 0)
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
            if(e.ColumnIndex == -1)
            {
                return;
            }

            if(e.RowIndex == 0)
            {
                Console.WriteLine("TODO sort column");
                // TODO header sort
            }
            else
            {
                if(SamplesDataGridView.Columns[e.ColumnIndex] == SamplesDataGridView.Columns["Override"])
                {
                    //SoundRomEditor.Instance.Project.Samples[e.RowIndex].ToggleOverride();
                }
                else
                {
                    SoundRomEditor.Instance.Project.Samples[e.RowIndex].Play(false);
                }
            }
        }

        private void OnSamplesDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                // not sure needed this is the header row
            }
            else
            {
                // don't want to toggle override in here, since it double toggles as we're accepting
                // click on the whole cell:
                if (SamplesDataGridView.Columns[e.ColumnIndex] == SamplesDataGridView.Columns["Override"])
                {
                    SoundRomEditor.Instance.Project.Samples[e.RowIndex].ToggleOverride();
                }
            }
        }

    }
}
