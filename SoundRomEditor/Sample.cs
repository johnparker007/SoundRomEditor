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

        public int Rate
        {
            get;
            private set;
        }

        public byte[] RawWavBytes
        {
            get;
            private set;
        }


        public Sample(byte[] rawWaveBytes, int rate)
        {
            RawWavBytes = rawWaveBytes;
            Rate = rate;
        }

        public void Play(bool loop)
        {
            // TODO this will want to be on a thread I think
            //var ms = new MemoryStream(raw);
            //var ms = new MemoryStream(SourceRomByteArrays[0]);
            var ms = new MemoryStream(RawWavBytes);
            var rs = new RawSourceWaveStream(ms, new WaveFormat(Rate, 16, 1));


            var wo = new WaveOutEvent();
            wo.Init(rs);
            wo.Play();
            while (wo.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(500);
            }
            wo.Dispose();
        }

        public void Stop()
        {

        }
    }
}
