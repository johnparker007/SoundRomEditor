using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor.ViewModels
{
    public class ViewModelMainForm
    {
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
    }
}
