using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoundRomEditor;

namespace SoundRomEditor.Codecs
{
    public abstract class Codec
    {
        public abstract List<Sample> DecodeRoms(List<byte[]> soundRomsData);
        public abstract List<byte[]> EncodeRoms(List<Sample> samples);

        protected List<byte> GetMergedRomData(List<byte[]> sourceRomsData)
        {
            List<byte> mergedRomData = new List<byte>();
            foreach (byte[] sourceRomData in sourceRomsData)
            {
                mergedRomData.AddRange(sourceRomData);
            }

            return mergedRomData;
        }
    }
}
