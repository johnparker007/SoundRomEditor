using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundRomEditor
{
    public class Sample
    {
        public class SampleData
        {
            public SampleData(byte[] originalData, byte[] linearPCM, int rate)
            {
                Rate = rate;
                OriginalData = originalData;
                LinearPCM = linearPCM;
            }

            public byte[] OriginalData
            {
                get;
                private set;
            }

            public byte[] LinearPCM
            {
                get;
                private set;
            }

            public int Rate
            {
                get;
                private set;
            }

            public int SampleCount
            {
                get
                {
                    // TODO this is where I'll need something like Sample base class
                    // with subclasses like SampleADPCM, SampleNEC, SampleYMZ etc and
                    // reference that source data rather than the Windows-native linear PCM 

                    // for now just hardcode the "/2" which gives us the adpcm byte sample count
                    if (LinearPCM == null)
                    {
                        return 0;
                    }
                    return LinearPCM.Length / 2;
                }
            }

            public float Duration
            {
                get
                {
                    if (Rate == 0)
                    {
                        return 0;
                    }

                    return (float)SampleCount / Rate;
                }
            }
        }

        public const int kBits = 16;
        public const int kChannels = 1; // TODO stereo sample support

        public SampleData OriginalSampleData
        {
            get;
            private set;
        }

        public SampleData OverrideSampleData
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public int SampleCount
        {
            get
            {
                return OriginalSampleData.SampleCount;
            }
        }

        public int Rate
        {
            get
            {
                return OriginalSampleData.Rate;
            }
        }

        public float Duration
        {
            get
            {
                return OriginalSampleData.Duration;
            }
        }
 
        public bool Override
        {
            get;
            private set;
        }

        protected SampleData ActiveSampleData
        {
            get
            {
                if (Override && OverrideSampleData != null)
                {
                    return OverrideSampleData;
                }
                else
                {
                    return OriginalSampleData;
                }
            }
        }


        public Sample(int index, byte[] originalData, byte[] linearPCM, int rate)
        {
            Index = index;
            OriginalSampleData = new SampleData(originalData, linearPCM, rate);
        }

        public void SetOverrideLinearPCM(byte[] linearPCM, int rate)
        {
            OverrideSampleData = new SampleData(null, linearPCM, rate);
        }

        public void Play(bool loop)
        {
            // TODO this will want to be on a thread rather than blocking app until completed

            WaveOutEvent waveOutEvent = new WaveOutEvent();
            waveOutEvent.Init(GetRawSourceWaveStream(ActiveSampleData));
            waveOutEvent.Play();
            while (waveOutEvent.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1);
            }
            waveOutEvent.Dispose();
        }

        public void Stop()
        {

        }

        public void SaveWav(string filename)
        {
            WaveFileWriter.CreateWaveFile(filename, GetRawSourceWaveStream(ActiveSampleData));
        }

        public void ToggleOverride()
        {
            Override = !Override;
        }

        private RawSourceWaveStream GetRawSourceWaveStream(SampleData sampleData)
        {
            MemoryStream memoryStream = new MemoryStream(sampleData.LinearPCM);
            RawSourceWaveStream rawSourceWaveStream = 
                new RawSourceWaveStream(memoryStream, new WaveFormat(sampleData.Rate, kBits, kChannels));

            return rawSourceWaveStream;
        }
    }
}
