using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor.Codecs
{
    public class CodecNEC : Codec
    {
        public override List<Sample> DecodeRoms(List<byte[]> soundRomsData)
        {
            throw new NotImplementedException();
        }

        public override List<byte[]> EncodeRoms(List<Sample> samples)
        {
            throw new NotImplementedException();
        }
    }
}
