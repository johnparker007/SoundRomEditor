using System;
using System.Collections.Generic;
using System.Linq;

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
            // build rom samples data
            List<List<byte>> romSamplesData = new List<List<byte>>();
            for (int sampleIndex = 0; sampleIndex < kSampleCount; ++sampleIndex)
            {
                List<byte> romSampleData = null;
                Sample sample = samples[sampleIndex];
                if(sample != null && sample.OriginalSampleData.OriginalData != null)
                {
                    List<byte> adpcmData;
                    if(sample.Override && sample.OverrideSampleData != null)
                    {
                        // TOIMPROVE - most/all of this Adpcm class should prob just be static?
                        Adpcm adpcm = new Adpcm(null, Adpcm.InputDataFormat.LinearPcm);

                        adpcmData = adpcm.EncodeFromLinearPCM(sample.OverrideSampleData.LinearPCM).ToList();
                    }
                    else
                    {
                        adpcmData = samples[sampleIndex].OriginalSampleData.OriginalData.ToList();
                    }

                    romSampleData = EncodeAdpcmDataToRomData(adpcmData);
                }

                romSamplesData.Add(romSampleData);
            }

            // build rom samples header
            List<byte> romHeaderData = BuildHeaderRomData(samples, romSamplesData);

            // build total rom data
            List<byte> totalRomData = new List<byte>();
            totalRomData.AddRange(romHeaderData);
            // need to add the unknown 64 byte area, just fill with zeroes for now
            const int kUnknownByteAreaLength = 64;
            totalRomData.AddRange(new byte[kUnknownByteAreaLength]);
            // now append all the rom samples data
            foreach(List<byte> romSampleData in romSamplesData)
            {
                if(romSampleData != null)
                {
                    totalRomData.AddRange(romSampleData);
                }
            }

            // split into 512kb roms (TODO this may be different for different MFME platforms?
            const int kSoundRomCapacity = 512 * 1024;
            List<byte[]> soundRomsData = new List<byte[]>();
            int readPosition = 0;
            while(readPosition < totalRomData.Count)
            {
                List<byte> romData = new List<byte>();
                int remainingToRead = totalRomData.Count - readPosition;
                int readLength = Math.Min(remainingToRead, kSoundRomCapacity);
                romData.AddRange(totalRomData.GetRange(readPosition, readLength));
                soundRomsData.Add(romData.ToArray());

                readPosition += readLength;
            }

            // fix final rom length with padding bytes
            List<byte> finalRom = new List<byte>();
            finalRom.AddRange(soundRomsData.Last());
            int paddingBytesRequired = kSoundRomCapacity - soundRomsData.Last().Length;
            byte[] paddingBytes = new byte[paddingBytesRequired];
            for (int i = 0; i < paddingBytes.Length; i++)
            {
                const byte kPaddingByte = 0xff; // TODO actually set the padding bytes to this, pad  may be zeroes or something else on other platforms
                paddingBytes[i] = kPaddingByte;
            }
            finalRom.AddRange(paddingBytes);
            soundRomsData[soundRomsData.Count - 1] = finalRom.ToArray();

            return soundRomsData;
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
            if (sampleAddress == 0)
            {
                Sample nullSample = new Sample(sampleIndex, null, null, 0);
                Console.WriteLine("Sample# " + sampleIndex + "    null");
                return nullSample;
            }

            List<byte> sampleAdpcmData = DecodeAdpcmDataFromRomData(mergedRomData, sampleAddress);
            Adpcm adpcm = new Adpcm(sampleAdpcmData.ToArray(), Adpcm.InputDataFormat.Adpcm);
            byte[] linearPCM = adpcm.DecodeToLinearPCM();
            int sampleRate = GetSampleRate(mergedRomData, headerAddress);
            Sample sample = new Sample(sampleIndex, sampleAdpcmData.ToArray(), linearPCM, sampleRate);

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

            int address = GetAddressFromHeaderData(headerData);

            return address;
        }

        private int GetAddressFromHeaderData(byte[] headerData)
        {
            int address = (headerData[0] << 16) + (headerData[1] << 8) + headerData[2];
            address &= 0x1fffff;

            return address;
        }

        private int GetSampleRate(List<byte> mergedRomData, int headerAddress)
        {
            byte[] headerData = GetHeaderData(mergedRomData, headerAddress);

            int rate = GetRateFromHeaderData(headerData);

            return rate;
        }

        private int GetRateFromHeaderData(byte[] headerData)
        {
            int rateKeyFromRomHeader = (16 - ((headerData[0] & 0xc0) >> 4)) * 1000;
            // JP bit of a guess from the MSM6376 datasheet:
            Dictionary<int, int> frequenciesLookup = new Dictionary<int, int>()
            {
                // rom key, frequency
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

        private List<byte> BuildHeaderRomData(List<Sample> samples, List<List<byte>> romSamplesData)
        {
            List<byte> headerData = new List<byte>();

            // TODO need to look into this more, is it always 0220?  Should it be extracted
            // from the source ROM header?
            const int kRomSampleDataStartAddress = 0x0220;
            int sampleDataAddress = kRomSampleDataStartAddress;
            for (int sampleIndex = 0; sampleIndex < kSampleCount; ++sampleIndex)
            {
                Sample sample = samples[sampleIndex];
                List<byte> romSampleData = romSamplesData[sampleIndex];

                byte[] sampleHeaderData = BuildPerSampleHeaderData(sample, romSampleData, sampleDataAddress);
                headerData.AddRange(sampleHeaderData);

                if(romSampleData != null)
                {
                    sampleDataAddress += romSampleData.Count;
                }
            }

            return headerData;
        }

        private byte[] BuildPerSampleHeaderData(Sample sample, List<byte> romSampleData, int sampleDataAddress)
        {
            byte[] headerData = new byte[kHeaderSampleInfoSize];
            if (sample == null || sample.OriginalSampleData.OriginalData == null)
            {
                return headerData;
            }
            
            // JP bit of a guess from the MSM6376 datasheet:
            Dictionary<int, int> rateKeyLookup = new Dictionary<int, int>()
            {
                // frequency, rom key
                { 4000, 4000 },
                { 6400, 6000 },
                { 8000, 8000 },
                { 12800, 12000 },
                { 16000, 16000 }
            };

            int rateKey = rateKeyLookup[sample.Rate]; // TODO this will be potentially different for overrides!

            // set address into header bytes
            headerData[0] = (byte)((sampleDataAddress >> 16) & 0xff);
            headerData[1] = (byte)((sampleDataAddress >> 8) & 0xff);
            headerData[2] = (byte)((sampleDataAddress >> 0) & 0xff);

            // OR the rate key into zeroth byte
            headerData[0] |= (byte)(((16 - (rateKey / 1000)) << 4) & 0xc0);

            return headerData;
        }

        private List<byte> DecodeAdpcmDataFromRomData(List<byte> mergedRomData, int sampleAddress)
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

        private List<byte> EncodeAdpcmDataToRomData(List<byte> adpcmData)
        {
            List<byte> romData = new List<byte>();
            int readPosition = 0;

            while(readPosition < adpcmData.Count)
            {
                WriteNextBlock(adpcmData, ref readPosition, romData);
            }

            romData.Add(0);

            return romData;
        }

        private void WriteNextBlock(List<byte> adpcmData, ref int readPosition, List<byte> romData)
        {
            int nextBlockSize = Math.Min(adpcmData.Count - readPosition, 0x7f);

            byte nextBlockSizeByte = (byte)nextBlockSize; // TODO not sure how the 0x7f are 0xff, but lesser ones are not...
            nextBlockSizeByte |= 0x80; // is this how to match original?
            romData.Add(nextBlockSizeByte);

            for (int nextBlockIndex = 0; nextBlockIndex < nextBlockSize; ++nextBlockIndex)
            {
                romData.Add(adpcmData[readPosition + nextBlockIndex]);
            }

            readPosition += nextBlockSize;
        }


    }
}
