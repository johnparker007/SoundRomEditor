using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoundRomEditor.ViewModels;

namespace SoundRomEditor
{
    public class SoundRomEditor
    {
        public static SoundRomEditor Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SoundRomEditor();
                }

                return _instance;
            }
        }

        public Project Project
        {
            get;
            private set;
        } = null;

        private static SoundRomEditor _instance = null;

        public ViewModelMainForm ViewModelMainForm
        {
            get;
            private set;
        }

        public SoundRomEditor()
        {
            BuildViewModels();

            Project = new Project();
        }

        private void BuildViewModels()
        {
            ViewModelMainForm = new ViewModelMainForm();
        }
    }
}
