using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoundRomEditor.Codecs;

namespace SoundRomEditor.Utility
{
    public static class CodecHelpers 
    {
        public static Type GetCodecType(string mfmePlatformString)
        {
            switch(mfmePlatformString)
            {
                case "MPU4":
                    return typeof(CodecOKI);
                default:
                    Console.WriteLine("ERROR!  Can't find MFME platform string: " + mfmePlatformString);
                    return null;
            }
        }

        public static Codec CreateCodec(string mfmePlatformString)
        {
            Type codecType = GetCodecType(mfmePlatformString);
            return CreateCodec(codecType);
        }

        public static Codec CreateCodec(Type codecType)
        {
            if (codecType == typeof(CodecOKI))
            {
                return new CodecOKI();
            }
            else if(codecType == typeof(CodecNEC))
            {
                return new CodecNEC();
            }
            else if (codecType == typeof(CodecYMZ))
            {
                return new CodecYMZ();
            }
            else
            {
                Console.WriteLine("ERROR!  Can't find codec type: " + codecType);
                return null;
            }
        }
    }
}
