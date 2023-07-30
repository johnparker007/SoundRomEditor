using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor.Codecs
{
    public class CodecOKI : Codec
    {
        public const int kSampleCount = 120; // TODO banked roms for 240 sounds
        public const int kHeaderSampleInfoSize = 4;

        public override List<Sample> DecodeRoms(List<byte[]> soundRomsData)
        {
            List<byte> mergedRomData = GetMergedRomData(soundRomsData);

            List<Sample> samples = ExtractSamples(mergedRomData);

            return samples;
        }

        public override List<byte[]> EncodeRoms(List<Sample> samples)
        {
            throw new NotImplementedException();
        }

        private List<Sample> ExtractSamples(List<byte> mergedRomData)
        {
            List<Sample> samples = new List<Sample>();

            for (int sampleIndex = 0; sampleIndex < kSampleCount; ++sampleIndex)
            {
                samples.Add(ExtractSample(mergedRomData, sampleIndex));
            }

            return samples;
        }

        private Sample ExtractSample(List<byte> mergedRomData, int sampleIndex)
        {
            int headerAddress = sampleIndex * kHeaderSampleInfoSize;
            int sampleAddress = GetSampleAddress(mergedRomData, headerAddress);
            if(sampleAddress == 0)
            {
                Console.WriteLine("Sample# " + sampleIndex + "    null");
                return null;
            }

            List<byte> sampleAdpcmData = GetSampleAdpcmData(mergedRomData, sampleAddress);
            Adpcm adpcm = new Adpcm(sampleAdpcmData.ToArray());
            byte[] linearPCM = adpcm.DecodeToLinearPCM(); // TODO should this author the wav header in here?
            int sampleRate = GetSampleRate(mergedRomData, headerAddress);
            Sample sample = new Sample(linearPCM, sampleRate);

            Console.WriteLine("Sample# " + sampleIndex
                + "   headerAddress: " + headerAddress
                + "   sampleAddress: " + sampleAddress
                + "   sampleAdpcmData length: " + sampleAdpcmData.Count
                + "   => *2 : " + (sampleAdpcmData.Count * 2)
                + "   sampleRate: " + sampleRate);

            return sample;
        }

        private int GetSampleAddress(List<byte> mergedRomData, int headerAddress)
        {
            byte[] headerData = GetHeaderData(mergedRomData, headerAddress);
            int address = (headerData[0] << 16) + (headerData[1] << 8) + headerData[2];
            address &= 0x1fffff;

            return address;
        }

        private int GetSampleRate(List<byte> mergedRomData, int headerAddress)
        {
            byte[] headerData = GetHeaderData(mergedRomData, headerAddress);
            int rateKeyFromRomHeader = (16 - ((headerData[0] & 0xc0) >> 4)) * 1000;
            // JP bit of a guess from the MSM6376 datasheet:
            Dictionary<int, int> frequenciesLookup = new Dictionary<int, int>() 
            { 
                { 4000, 4000 },
                { 6000, 6400 },
                { 8000, 8000 },
                { 12000, 12800 },
                { 16000, 16000 }
            };

            return frequenciesLookup[rateKeyFromRomHeader];
        }

        private byte[] GetHeaderData(List<byte> mergedRomData, int headerAddress)
        {
            byte[] headerData = new byte[kHeaderSampleInfoSize];
            Array.Copy(mergedRomData.ToArray(), headerAddress, headerData, 0, kHeaderSampleInfoSize);

            return headerData;
        }

        private List<byte> GetSampleAdpcmData(List<byte> mergedRomData, int sampleAddress)
        {
            int readAddress = sampleAddress;
            List<byte> sampleAdpcmData = new List<byte>();

            byte[] nextBlockData = ReadNextBlock(mergedRomData, ref readAddress);
            do
            {
                sampleAdpcmData.AddRange(nextBlockData);
                nextBlockData = ReadNextBlock(mergedRomData, ref readAddress);
            }
            while (readAddress < mergedRomData.Count && nextBlockData != null);

            return sampleAdpcmData;            
        }

        private byte[] ReadNextBlock(List<byte> mergedRomData, ref int readAddress)
        {
            byte blockLength = mergedRomData[readAddress];
            blockLength &= 0x7f;
            if(blockLength == 0)
            {
                return null;
            }
            ++readAddress;

            byte[] blockData = new byte[blockLength];
            Array.Copy(mergedRomData.ToArray(), readAddress, blockData, 0, blockLength);
            readAddress += blockLength;

            return blockData;
        }
    }
}
