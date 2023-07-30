using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor
{
    public class Adpcm
    {
        private static readonly short[] kStepSize = 
        { 
            16, 17, 19, 21, 23, 25, 28, 31, 34, 37, 41,
            45, 50, 55, 60, 66, 73, 80, 88, 97, 107, 118, 130, 143, 157, 173,
            190, 209, 230, 253, 279, 307, 337, 371, 408, 449, 494, 544, 598, 658,
            724, 796, 876, 963, 1060, 1166, 1282, 1408, 1552 
        };

        private byte[] _data = null;
        private short _index = 0;
        private short _last = 0;


        public Adpcm(byte[] data)
        {
            _data = data;

            Initialise();
        }


        //---------------------------------------------------------------------------
        /*
        * Initialze the data used by the coder.
        */
        //        void __fastcall Sample::adpcm_init( struct adpcm_status *stat )
        //{
        //    stat->last = 0;
        //    stat->step_index = 0;
        //}

        public void Initialise()
        {
            _last = 0;
            _index = 0;
        }
    //---------------------------------------------------------------------------
    /*
    * Encode linear to ADPCM
    */
//    char __fastcall Sample::adpcm_encode(short samp, struct adpcm_status * stat )
//{
//    short code;
//    short diff, E, SS;

//    SS = step_size[stat->step_index];
//    code = 0x00;
//    if((diff = samp - stat->last) < 0 )
//        code = 0x08;
//    E = diff< 0 ? -diff : diff;
//    if(E >= SS ) {
//        code = code | 0x04;
//        E -= SS;
//    }
//if (E >= SS / 2)
//{
//    code = code | 0x02;
//    E -= SS / 2;
//}
//if (E >= SS / 4)
//{
//    code = code | 0x01;
//}
///*    stat->step_index += step_adjust( code );
//    if( stat->step_index < 0 ) stat->step_index = 0;
//    if( stat->step_index > 48 ) stat->step_index = 48;
//*/
///*
//* Use the decoder to set the estimate of last sample.
//* It also will adjust the step_index for us.
//*/
//stat->last = adpcm_decode(code, stat);
//return (code);
//}


        public byte[] DecodeToWav()
        {
            List<byte> output = new List<byte>();

            for (int adpcmReadIndex = 0; adpcmReadIndex < _data.Length; ++adpcmReadIndex)
            {
                byte sourceAdpcmByte = _data[adpcmReadIndex];
                short decoded = Decode((byte)((sourceAdpcmByte >> 4) & 0xf));
                output.Add((byte)(decoded & 0xff));
                output.Add((byte)(decoded >> 8));

                decoded = Decode((byte)(sourceAdpcmByte & 0xf));
                output.Add((byte)(decoded & 0xff));
                output.Add((byte)(decoded >> 8));
            }

            return output.ToArray();
        }

        public short Decode(byte code)
        {
            short diff, E, SS, sample;

            SS = kStepSize[_index];
            E = (short)(SS / 8);
            if ((code & 0x01) > 0)
            {
                E += (short)(SS >> 2);
            }
            if ((code & 0x02) > 0)
            {
                E += (short)(SS >> 1);
            }
            if ((code & 0x04) > 0)
            {
                E += SS;
            }
            diff = ((code & 0x08) > 0) ? (short)-E : E;
            sample = (short)(_last + diff);

            /*
            *  Clip the values to +/- 2^11 (supposed to be 12 bits)
            */
            if (sample > 2047) sample = 2047;
            if (sample < -2048) sample = -2048;

            _last = sample;
            _index += StepAdjust(code);
            if (_index < 0) _index = 0;
            if (_index > 48) _index = 48;

            return sample;
        }


        //short OKIDecodeNibble(byte code, adpcm_status stat)
        //{
        //    short SS, Sample, Diff, E;


        //    SS = step_size[stat.step_index];
        //    E = (short)(SS / 8);

        //    if ((Nibble & 0x01) > 0)
        //    {
        //        E += (short)(SS >> 2);
        //    }
        //    if ((Nibble & 0x02) > 0)
        //    {
        //        E += (short)(SS >> 1);
        //    }
        //    if ((Nibble & 0x04) > 0)
        //    {
        //        E += (short)SS;
        //    }

        //    if ((Nibble & 0x08) > 0)
        //    {
        //        Diff = (short)-E;
        //    }
        //    else
        //    {
        //        Diff = (short)E;
        //    }

        //    Sample = (short)(ADPCMLast + Diff);

        //    if (Sample > 2047)
        //    {
        //        Sample = 2047;
        //    }
        //    if (Sample < -2048)
        //    {
        //        Sample = -2048;
        //    }

        //    ADPCMLast = Sample;
        //    ADPCMIndex += OKIAdjusts[Nibble];

        //    if (ADPCMIndex > 48)
        //    {
        //        ADPCMIndex = 48;
        //    }
        //    if (ADPCMIndex < 0)
        //    {
        //        ADPCMIndex = 0;
        //    }

        //    return (Sample);

        //}




        //---------------------------------------------------------------------------
        /*
        * adjust the step for use on the next sample.
        */
        //short __fastcall Sample::step_adjust ( char code )
        //{
        //    short reply;

        //    switch (code & 0x07)
        //    {
        //        case 0x00:
        //            reply = -1;
        //            break;
        //        case 0x01:
        //            reply = -1;
        //            break;
        //        case 0x02:
        //            reply = -1;
        //            break;
        //        case 0x03:
        //            reply = -1;
        //            break;
        //        case 0x04:
        //            reply = 2;
        //            break;
        //        case 0x05:
        //            reply = 4;
        //            break;
        //        case 0x06:
        //            reply = 6;
        //            break;
        //        case 0x07:
        //            reply = 8;
        //            break;
        //    }
        //    return reply;
        //}

        private static short StepAdjust(byte code)
        {
            short reply = 0;

            switch (code & 0x07)
            {
                case 0x00:
                    reply = -1;
                    break;
                case 0x01:
                    reply = -1;
                    break;
                case 0x02:
                    reply = -1;
                    break;
                case 0x03:
                    reply = -1;
                    break;
                case 0x04:
                    reply = 2;
                    break;
                case 0x05:
                    reply = 4;
                    break;
                case 0x06:
                    reply = 6;
                    break;
                case 0x07:
                    reply = 8;
                    break;
            }

            return reply;
        }













    }
}
