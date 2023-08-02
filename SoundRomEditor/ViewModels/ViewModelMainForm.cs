using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor.ViewModels
{
    public class ViewModelMainForm
    {
        private ViewMainForm _viewMainForm = null;

        public Sample SelectedSample
        {
            get
            {
                return _viewMainForm.SelectedSample;
            }
        }

        public void SetViewMainForm(ViewMainForm viewMainForm)
        {
            _viewMainForm = viewMainForm;
        }

        public void PlayAllSamples()
        {
            SoundRomEditor.Instance.Project.PlayAllSamples();
        }

        public void SaveAllWavs()
        {
            SoundRomEditor.Instance.Project.SaveAllWavs();
        }

        public void LoadRoms()
        {
            SoundRomEditor.Instance.Project.LoadRomsFromSelector();
        }

        public void SaveRoms()
        {
            SoundRomEditor.Instance.Project.SaveRoms("SRE_SOUND_ROM");
        }

        public void LoadWav()
        {
            SoundRomEditor.Instance.Project.LoadWavFromSelector();
        }
    }
}
