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
        }

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
                LoadRoms(openFileDialog.FileNames);
            }
        }

        public void SaveRoms(string baseFilename)
        {
            Codec codec = CodecHelpers.CreateCodec(kTempMfmePlatformString);
            List<byte[]> roms = codec.EncodeRoms(Samples);
            for (int romIndex = 0; romIndex < roms.Count; ++romIndex)
            {
                File.WriteAllBytes(baseFilename + romIndex, roms[romIndex]);
            }
        }

        public void LoadWavFromSelector()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open WAV",
                Filter = "Wav files (*.wav)|*.wav",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadWav(openFileDialog.FileName);
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

        private void SaveWav(int sampleIndex)
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

        private void LoadRoms(string[] sourceRomPaths)
        {
            SourceRomPaths = new List<string>();
            SourceRomPaths.AddRange(sourceRomPaths);

            SourceRomBytes = new List<byte[]>();
            foreach (string sourceRomPath in sourceRomPaths)
            {
                SourceRomBytes.Add(File.ReadAllBytes(sourceRomPath));
            }

            ExtractSamples();

            OnLoadRomsCompleted?.Invoke();
        }

        private void LoadWav(string wavPath)
        {
            byte[] sourceWavBytes = File.ReadAllBytes(wavPath);
            // TOIMPROVE don't convert if wav just happens to be mono/sterio we nbeed and bitrate we need
            byte[] convertedLinearPCM = ConvertWavTo16000Hz16BitMonoWav(sourceWavBytes, false);

            SoundRomEditor.Instance.ViewModelMainForm.SelectedSample.SetOverrideLinearPCM(
                convertedLinearPCM, 16000); // TODO magic number
        }

        // TODO fixed, needs to be codec-specific, with potential user override for Hz
        public static byte[] ConvertWavTo16000Hz16BitMonoWav(byte[] inArray, bool writeHeader)
        {
            using (var mem = new MemoryStream(inArray))
            {
                using (var reader = new WaveFileReader(mem))
                {
                    using (var converter = WaveFormatConversionStream.CreatePcmStream(reader))
                    {
                        using (var upsampler = new WaveFormatConversionStream(new WaveFormat(16000, 16, 1), converter))
                        {
                            byte[] data;
                            using (var m = new MemoryStream())
                            {
                                upsampler.CopyTo(m);
                                data = m.ToArray();
                            }

                            if(writeHeader)
                            {
                                using (var m = new MemoryStream())
                                {
                                    // to create a propper WAV header (44 bytes), which begins with RIFF 
                                    var w = new WaveFileWriter(m, upsampler.WaveFormat);
                                    // append WAV data body
                                    w.Write(data, 0, data.Length);
                                    return m.ToArray();
                                }
                            }
                            else
                            {
                                return data;
                            }

                        }
                    }
                }
            }
        }

    }
}
