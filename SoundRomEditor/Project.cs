using NAudio.Wave;
using SoundRomEditor.Codecs;
using SoundRomEditor.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SoundRomEditor
{
    public class Project
    {
        public const string kTempMfmePlatformString = "MPU4";// TODO - TEMP!

        public Action OnLoadRomsCompleted = null;

        public List<string> SourceRomPaths
        {
            get;
            private set;
        } = new List<string>();

        public List<byte[]> SourceRomBytes
        {
            get;
            private set;
        }

        public List<Sample> Samples
        {
            get;
            private set;
        }

        public Project()
        {
        }

        public void LoadRoms()
        {
            // TODO check mfme source, I think it sorts the sound roms by filename before loading

            SourceRomBytes = new List<byte[]>();
            foreach (string sourceRomPath in SourceRomPaths)
            {
                SourceRomBytes.Add(File.ReadAllBytes(sourceRomPath));
            }

            ExtractSamples();

            OnLoadRomsCompleted?.Invoke();
        }

        public void LoadRomsFromSelector()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Sound ROMs",
                Filter = "All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SourceRomPaths.Clear();
                SourceRomPaths.AddRange(openFileDialog.FileNames);

                LoadRoms();
            }
        }

        public void PlayAllSamples()
        {
            Console.WriteLine("PlayAllSamples");

            for(int sampleIndex = 0; sampleIndex < Samples.Count; ++sampleIndex)
            {
                PlaySample(sampleIndex);
            }
        }

        public void PlaySample(int sampleIndex)
        {
            Sample sample = Samples[sampleIndex];
            if (sample == null)
            {
                Console.WriteLine("Skipping empty sample #" + sampleIndex);
            }
            else
            {
                Console.WriteLine("Playing sample #" + sampleIndex);

                Samples[sampleIndex].Play(false);
            }
        }

        public void SaveAllWavs()
        {
            Console.WriteLine("SaveAllWavs");

            for (int sampleIndex = 0; sampleIndex < Samples.Count; ++sampleIndex)
            {
                SaveWav(sampleIndex);
            }
        }

        public void SaveWav(int sampleIndex)
        {
            Sample sample = Samples[sampleIndex];
            if (sample == null)
            {
                Console.WriteLine("Skipping empty sample #" + sampleIndex);
            }
            else
            {
                Console.WriteLine("Saving wav of sample #" + sampleIndex);
                Samples[sampleIndex].SaveWav(@"sample" + sampleIndex + ".wav");
            }
        }

        private void ExtractSamples()
        {
            Codec codec = CodecHelpers.CreateCodec(kTempMfmePlatformString);
            Samples = codec.DecodeRoms(SourceRomBytes);
        }
    }
}
