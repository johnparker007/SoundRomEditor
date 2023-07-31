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
        public const int kBits = 16;
        public const int kChannels = 1; // TODO stereo sample support

        public int Index
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
                if(LinearPCM == null)
                {
                    return 0;
                }
                return LinearPCM.Length / 2; 
            }
        }

        public int Rate
        {
            get;
            private set;
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

        protected byte[] LinearPCM
        {
            get;
            private set;
        }



        public Sample(int index, byte[] linearPCM, int rate)
        {
            Index = index;
            LinearPCM = linearPCM;
            Rate = rate;
        }

        public void Play(bool loop)
        {
            // TODO this will want to be on a thread rather than blocking app until completed

            WaveOutEvent waveOutEvent = new WaveOutEvent();
            waveOutEvent.Init(GetRawSourceWaveStream());
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
            WaveFileWriter.CreateWaveFile(filename, GetRawSourceWaveStream());
        }

        private RawSourceWaveStream GetRawSourceWaveStream()
        {
            MemoryStream memoryStream = new MemoryStream(LinearPCM);
            RawSourceWaveStream rawSourceWaveStream = 
                new RawSourceWaveStream(memoryStream, new WaveFormat(Rate, kBits, kChannels));

            return rawSourceWaveStream;
        }
    }
}
