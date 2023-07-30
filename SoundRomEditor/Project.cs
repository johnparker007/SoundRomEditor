using NAudio.Wave;
using SoundRomEditor.Codecs;
using SoundRomEditor.Utility;
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
        public const string kTempMfmePlatformString = "MPU4";// TODO - TEMP!

        public const int kTempFixedRate = 8000; // TODO - TEMP!
        public const int kSampleCount = 120; // at least for the standard MPU4 routine, this is fixed

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

// this prob wants to happen inside codec
public List<byte> SourceRomsBytes
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
            SourceRomPaths = new List<string>();
            SourceRomsBytes = new List<byte>();

            // test:
            SourceRomPaths.Add(@"C:\projects\Arcade_SourceLayouts\LegacySectionFromDif\Unzipped\Barcrest\Andy Capp (Barcrest)\andsnd.p1");
            SourceRomPaths.Add(@"C:\projects\Arcade_SourceLayouts\LegacySectionFromDif\Unzipped\Barcrest\Andy Capp (Barcrest)\andsnd.p2");

            //SourceRomPaths.Add(@"C:\projects\Arcade_SourceLayouts\LegacySectionFromDif\Unzipped\JPM\Indiana Jones (JPM)\6706.bin");

            // also test:
            LoadRoms();
            BuildSamplesNew2();
        }

        public void LoadRoms()
        {
            // TODO check mfme source, I think it sorts the sound roms by filename before loading

            SourceRomBytes = new List<byte[]>();
            foreach (string sourceRomPath in SourceRomPaths)
            {
                SourceRomBytes.Add(File.ReadAllBytes(sourceRomPath));
            }
        }

        

        public void SaveWav()
        {
            Console.WriteLine("SaveWav");

            //BuildSamples();

// test:
            for(int sampleIndex = 0; sampleIndex < Samples.Count; ++sampleIndex)
            {
                Sample sample = Samples[sampleIndex];
                if(sample == null)
                {
                    Console.WriteLine("Skipping empty sample #" + sampleIndex);
                }
                else
                {
                    Console.WriteLine("Playing sample #" + sampleIndex);

                    File.WriteAllBytes(@"sample" + sampleIndex + ".raw", Samples[sampleIndex].RawWavBytes);

                    Samples[sampleIndex].Play(false);
                }
            }

return;

            Adpcm adpcm = new Adpcm(SourceRomsBytes.ToArray());
            byte[] wav = adpcm.DecodeToWav();



            var sampleRate = 8000;
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

        private void BuildSamplesNew2()
        {
            Codec codec = CodecHelpers.CreateCodec(kTempMfmePlatformString);
            Samples = codec.DecodeRoms(SourceRomBytes);
        }

        private void BuildSamplesNEW()
        {
            for(int sampleIndex = 0; sampleIndex < kSampleCount; ++sampleIndex)
            {
                BuildSample(sampleIndex);
            }
        }

        private void BuildSample(int sampleIndex)
        {
            int address = 0, Size, loop;
            //short sample;
            byte value, length;
            byte[] bp = new byte[3];
            int Reallength;



            int soundPosition = sampleIndex * 4;
            Array.Copy(SourceRomsBytes.ToArray(), soundPosition, bp, 0, 3);
            address = (bp[0] << 16) + (bp[1] << 8) + bp[2];
            // TODO set up rate
            address &= 0x1fffff;
            Size = 0;
            Reallength = 0;

            int sourceAdpcmDataLength = SourceRomsBytes[address];



            //Sample sampleInstance = new Sample();

            Console.WriteLine("sampleIndex == " + sampleIndex + " ... address == " + address);
        }

        public void BuildSamples()
        {
            //Adpcm adpcm = new Adpcm(SourceRomByteArrays[0]);
            //byte[] wav = adpcm.DecodeToWav();

            //            void __fastcall Sample::LoadSound(TStringList * SoundList, int volume, int Rate)
            //{
            //                TMemoryStream* file_ptr, *ptr;
            //                int count;
            //                unsigned char bp[3];
            //                int address, Size, loop;
            //                short sample;
            //                unsigned char value, length;
            //struct adpcm_status stat;
            //int Reallength;
            //        int rate;

            //  if (SoundList->Count ) {
            //    file_ptr = new TMemoryStream();
            //        sound = new TMemoryStream();
            //        SoundList->Sort();
            //    for (count = 0; count<SoundList->Count; count++ ) {
            //      file_ptr->LoadFromFile(SoundList->Strings[count]);
            //        sound->CopyFrom(file_ptr, 0);
            //    }
            //    delete file_ptr;
            //    for (count = 0; count< 120; count++ ) {
            //      sound->Position = count* 4;
            //      sound->Read(bp, 3);
            //    address = (bp[0] << 16) + (bp[1] << 8) + bp[2];
            ////      rate = (16 - ((bp[0] & 0xc0) >> 4)) * 1000;
            //      rate = Rate;
            //      address &= 0x1fffff;
            //      Size = 0;
            //      Reallength = 0;
            //      ptr = new TMemoryStream();
            //      if (address > 0 && address<sound->Size ) {
            //        adpcm_init( &stat);
            //    sound->Position = address;
            //        sound->Read( &length, 1);
            //    Reallength = 1;
            //        length &= 0x7f;
            //        while (length  ) {
            //          for (loop = 0; loop<length; loop++ ) {
            //            sound->Read( &value, 1);
            //    sample = volume* adpcm_decode((value >> 4) & 0xF, &stat);
            //            ptr->Write( &sample, 2);
            //    sample = volume* adpcm_decode(value & 0xF, &stat);
            //    ptr->Write( &sample, 2);
            //    Size += 4;
            //            Reallength++;
            //          }
            //sound->Read(&length, 1);
            //length &= 0x7f;
            //Reallength++;
            ////          if ( length == 0xc4 )
            ////            printf("oh oh\n");
            //        }
            //      } else
            //{
            //    if (count == 0)
            //    {
            //        Size = 8;
            //        sample = 0;
            //        for (loop = 0; loop < Size / 2; loop++)
            //            ptr->Write(&sample, 2);
            //    }
            //}
            //RealLength[count] = Reallength;
            //if (Size > 0)
            //{
            //    Length[count] = Size;
            //    ptr->Position = 0;
            //    //        samples[count] = ptr;
            //    if (AppCreateBasicBuffer(lpds, &tune[count], rate ? rate : Rate, Size, true))
            //        AppWriteDataToBuffer(tune[count], 0, (unsigned char *)ptr->Memory, Size);
            //    rates[count] = rate ? rate : Rate;
            //}
            //else
            //    Length[count] = 0;
            //tunes[count] = ptr;
            ////      rates[count] = Rate;
            ////      delete ptr;
            //    }
            //    if (muted)
            //{
            //    muted = !muted;
            //    Mute();
            //}
            //  }
            //}

            int count;
            byte[] bp = new byte[3];
            int address = 0, Size, loop;
            short sample;
            byte value, length;

            int Reallength;
            int rate;

            for (count = 0; count < 120; count++)
            {
                int soundPosition = count * 4;
                Array.Copy(SourceRomsBytes.ToArray(), soundPosition, bp, 0, 3);
                address = (bp[0] << 16) + (bp[1] << 8) + bp[2];
                // TODO set up rate
                address &= 0x1fffff;
                Size = 0;
                Reallength = 0;

                Console.WriteLine("Sample #: " + count + " ... address == " + address);

                List<byte> ptr = new List<byte>();
                if (address > 0 && address < SourceRomsBytes.Count)
                {
                    //adpcm_init(&stat);
                    //sound->Position = address;
                    //sound->Read(&length, 1);
                    //Reallength = 1;
                    //length &= 0x7f;
                    //while (length)
                    //{
                    //    for (loop = 0; loop < length; loop++)
                    //    {
                    //        sound->Read(&value, 1);
                    //        sample = volume * adpcm_decode((value >> 4) & 0xF, &stat);
                    //        ptr->Write(&sample, 2);
                    //        sample = volume * adpcm_decode(value & 0xF, &stat);
                    //        ptr->Write(&sample, 2);
                    //        Size += 4;
                    //        Reallength++;
                    //    }
                    //    sound->Read(&length, 1);
                    //    length &= 0x7f;
                    //    Reallength++;
                    //}
                }
                else
                {
                    if (count == 0)
                    {
                        //Size = 8;
                        //sample = 0;
                        //for (loop = 0; loop < Size / 2; loop++)
                        //    ptr->Write(&sample, 2);
                    }
                }

            }



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
