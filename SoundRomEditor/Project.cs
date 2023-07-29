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
    public class Project
    {
        public List<string> SourceRomPaths
        {
            get;
            private set;
        }

        public List<byte[]> SourceRomByteArrays
        {
            get;
            private set;
        }

        public Project()
        {
            SourceRomPaths = new List<string>();
            SourceRomByteArrays = new List<byte[]>();

            // test:
            //SourceRomPaths.Add(@"C:\projects\Arcade_SourceLayouts\LegacySectionFromDif\Unzipped\Barcrest\Andy Capp (Barcrest)\andsnd.p1");
            SourceRomPaths.Add(@"C:\projects\Arcade_SourceLayouts\LegacySectionFromDif\Unzipped\JPM\Indiana Jones (JPM)\6706.bin");

            // also test:
            LoadRoms();
        }

        public void LoadRoms()
        {
            SourceRomByteArrays.Add(File.ReadAllBytes(SourceRomPaths[0]));
        }

        public void SaveWav()
        {
            Console.WriteLine("SaveWav");


            Adpcm adpcm = new Adpcm(SourceRomByteArrays[0]);
            byte[] wav = adpcm.DecodeToWav();



            var sampleRate = 4000;
            //var frequency = 500;
            //var amplitude = 0.2;
            //var seconds = 5;

            //var raw = new byte[sampleRate * seconds * 2];

            //var multiple = 2.0 * frequency / sampleRate;
            //for (int n = 0; n < sampleRate * seconds; n++)
            //{
            //    var sampleSaw = ((n * multiple) % 2) - 1;
            //    var sampleValue = sampleSaw > 0 ? amplitude : -amplitude;
            //    var sample = (short)(sampleValue * Int16.MaxValue);
            //    var bytes = BitConverter.GetBytes(sample);
            //    raw[n * 2] = bytes[0];
            //    raw[n * 2 + 1] = bytes[1];
            //}


            // TODO this will want to be on a thread I think
            //var ms = new MemoryStream(raw);
            //var ms = new MemoryStream(SourceRomByteArrays[0]);
            var ms = new MemoryStream(wav);
            var rs = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, 16, 1));


            var wo = new WaveOutEvent();
            wo.Init(rs);
            wo.Play();
            while (wo.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(500);
            }
            wo.Dispose();




        }


        //PlayerEx pl = new PlayerEx();

        //private static void PlayArray(PlayerEx pl)
        //{
        //    double fs = 8000; // sample freq
        //    double freq = 1000; // desired tone
        //    short[] mySound = new short[4000];
        //    for (int i = 0; i < 4000; i++)
        //    {
        //        double t = (double)i / fs; // current time
        //        mySound[i] = (short)(Math.Cos(t * freq) * (short.MaxValue));
        //    }
        //    IntPtr format = AudioCompressionManager.GetPcmFormat(1, 16, (int)fs);
        //    pl.OpenPlayer(format);
        //    byte[] mySoundByte = new byte[mySound.Length * 2];
        //    Buffer.BlockCopy(mySound, 0, mySoundByte, 0, mySoundByte.Length);
        //    pl.AddData(mySoundByte);
        //    pl.StartPlay();
        //}


    }
}
