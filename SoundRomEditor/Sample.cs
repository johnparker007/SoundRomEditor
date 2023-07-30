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

        public int Rate
        {
            get;
            private set;
        }

        public byte[] LinearPCM
        {
            get;
            private set;
        }


        public Sample(byte[] linearPCM, int rate)
        {
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
