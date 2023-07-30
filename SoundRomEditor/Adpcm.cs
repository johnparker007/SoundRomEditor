using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor
{
    public class Adpcm
    {
        public struct adpcm_status
        {
            public short last;
            public short step_index;
        };

        private adpcm_status _adpcmStatus = new adpcm_status();

        private static short[] step_size = 
        { 
            16, 17, 19, 21, 23, 25, 28, 31, 34, 37, 41,
            45, 50, 55, 60, 66, 73, 80, 88, 97, 107, 118, 130, 143, 157, 173,
            190, 209, 230, 253, 279, 307, 337, 371, 408, 449, 494, 544, 598, 658,
            724, 796, 876, 963, 1060, 1166, 1282, 1408, 1552 
        };

        private byte[] _adpcmData = null;


        public Adpcm(byte[] adpcmData)
        {
            _adpcmData = adpcmData;

            Initialise();

            // test:

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
            _adpcmStatus.last = 0;
            _adpcmStatus.step_index = 0;
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

//---------------------------------------------------------------------------
/*
* Decode Linear to ADPCM
*/
//short __fastcall Sample::adpcm_decode( char code, struct adpcm_status *stat )
//{
//    short diff, E, SS, samp;

//    SS = step_size[stat->step_index];
//    E = SS / 8;
//    if (code & 0x01)
//        E += SS / 4;
//    if (code & 0x02)
//        E += SS / 2;
//    if (code & 0x04)
//        E += SS;
//    diff = (code & 0x08) ? -E : E;
//    samp = stat->last + diff;

//    /*
//    *  Clip the values to +/- 2^11 (supposed to be 12 bits)
//    */
//    if (samp > 2048) samp = 2048;
//    if (samp < -2048) samp = -2048;

//    stat->last = samp;
//    stat->step_index += step_adjust(code);
//    if (stat->step_index < 0) stat->step_index = 0;
//    if (stat->step_index > 48) stat->step_index = 48;

//    return (samp);
//}
        public byte[] DecodeToWav()
        {
            List<byte> output = new List<byte>();

            for (int adpcmReadIndex = 0; adpcmReadIndex < _adpcmData.Length; ++adpcmReadIndex)
            {

                //            sound->Read( &value, 1);
                //    sample = volume* adpcm_decode((value >> 4) & 0xF, &stat);
                //            ptr->Write( &sample, 2);
                //    sample = volume* adpcm_decode(value & 0xF, &stat);
                //    ptr->Write( &sample, 2);
                //    Size += 4;
                byte sourceAdpcmByte = _adpcmData[adpcmReadIndex];
                short decoded = (short)(1 * Decode((byte)((sourceAdpcmByte >> 4) & 0xF), _adpcmStatus));
                output.Add((byte)((decoded >> 8) & 255));
                output.Add((byte)((decoded >> 0) & 255));

                decoded = (short)(1 * Decode((byte)(sourceAdpcmByte & 0xF), _adpcmStatus));
                output.Add((byte)((decoded >> 8) & 255));
                output.Add((byte)((decoded >> 0) & 255));






                // JP Original:
                //short decoded = Decode(_adpcmData[adpcmReadIndex], _adpcmStatus);

                //output[(adpcmReadIndex * 2) + 0] = (byte)((decoded >> 8) & 255);
                //output[(adpcmReadIndex * 2) + 1] = (byte)((decoded >> 0) & 255);
            }

            return output.ToArray();
        }

        public static short Decode(byte code, adpcm_status stat)
        {
            short diff, E, SS, samp;

            SS = step_size[stat.step_index];
            E = (short)(SS / 8);
            if ((code & 0x01) != 0)
            {
                E += (short)(SS / 4);
            }
            if ((code & 0x02) != 0)
            {
                E += (short)(SS / 2);
            }
            if ((code & 0x04) != 0)
            {
                E += SS;
            }
            diff = ((code & 0x08) != 0) ? (short)-E : E;
            samp = (short)(stat.last + diff);

            /*
            *  Clip the values to +/- 2^11 (supposed to be 12 bits)
            */
            if (samp > 2048) samp = 2048;
            if (samp < -2048) samp = -2048;

            stat.last = samp;
            stat.step_index += StepAdjust(code);
            if (stat.step_index < 0) stat.step_index = 0;
            if (stat.step_index > 48) stat.step_index = 48;

            return samp;
        }

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
