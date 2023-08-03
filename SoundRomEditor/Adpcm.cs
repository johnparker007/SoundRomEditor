using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundRomEditor
{
    public class Adpcm
    {
        public enum InputDataFormat
        {
            Adpcm,
            LinearPcm
        }

        public const int kVolume = 0xf;

        private static readonly short[] kStepSize = 
        { 
            16, 17, 19, 21, 23, 25, 28, 31, 34, 37, 41,
            45, 50, 55, 60, 66, 73, 80, 88, 97, 107, 118, 
            130, 143, 157, 173, 190, 209, 230, 253, 279, 
            307, 337, 371, 408, 449, 494, 544, 598, 658,
            724, 796, 876, 963, 1060, 1166, 1282, 1408, 
            1552 
        };

        private byte[] _data = null;
        private short _index = 0;
        private short _last = 0;


        public Adpcm(byte[] data, InputDataFormat inputDataFormat)
        {
            switch(inputDataFormat)
            {
                case InputDataFormat.Adpcm:
                    _data = data;
                    break;
                case InputDataFormat.LinearPcm:
                    break;
                default:
                    Console.WriteLine("Input data format not set up: " + inputDataFormat);
                    break;
            }
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

        public byte[] EncodeFromLinearPCM(byte[] linearPcm)
        {
            List<byte> output = new List<byte>();

            const int kLinearPcmBytesPerAdpcmByte = 4;
            for (int linearPcmReadIndex = 0; 
                linearPcmReadIndex < linearPcm.Length; 
                linearPcmReadIndex += kLinearPcmBytesPerAdpcmByte)
            {
                const int kZeroDCBiasSampleValue = 0x80; // TODO check this is true for linear pcm
                byte linearPcmByte0 = kZeroDCBiasSampleValue;
                byte linearPcmByte1 = kZeroDCBiasSampleValue;
                byte linearPcmByte2 = kZeroDCBiasSampleValue;
                byte linearPcmByte3 = kZeroDCBiasSampleValue;

                linearPcmByte3 = linearPcm[linearPcmReadIndex];
                if(linearPcmReadIndex + 1 < linearPcm.Length)
                {
                    linearPcmByte2 = linearPcm[linearPcmReadIndex + 1];
                }
                if (linearPcmReadIndex + 2 < linearPcm.Length)
                {
                    linearPcmByte1 = linearPcm[linearPcmReadIndex + 2];
                }
                if (linearPcmReadIndex + 3 < linearPcm.Length)
                {
                    linearPcmByte0 = linearPcm[linearPcmReadIndex + 3];
                }

                short sample0 = (short)((linearPcmByte0 << 8) | linearPcmByte1);
                short sample1 = (short)((linearPcmByte2 << 8) | linearPcmByte3);

                // JP swapping linearPcmByte1 << 8) | linearPcmByte0 gives just noise
                //short sample0 = (short)((linearPcmByte1 << 8) | linearPcmByte0);
                //short sample1 = (short)((linearPcmByte3 << 8) | linearPcmByte2);

                // JP guess:
                sample0 /= (kVolume);
                sample1 /= (kVolume);

                byte adpcmNibble0 = (byte)((Encode(sample0) & 0xf) << 4);
                byte adpcmNibble1 = (byte)(Encode(sample1) & 0xf);

                byte adpcmByte = (byte)(adpcmNibble0 | adpcmNibble1);

                output.Add(adpcmByte);
            }

            return output.ToArray();
        }

        public byte Encode(short sample)
        {
            short diff, E, SS;
            byte code;

            diff = (short)(sample - _last);

            SS = kStepSize[_index];

            E = (short)(Math.Abs((int)diff) * 8 / SS);

            code = 0;
            if (E >= 4)
            {
                code |= 0x04;
                E -= 4;
            }
            if (E >= 2)
            {
                code |= 0x02;
                E -= 2;
            }
            if (E >= 1)
            {
                code |= 0x01;
            }
            if (diff < 0)
            {
                code |= 0x08;
            }

            _last = sample;
            _index += StepAdjust(code);
            if (_index < 0) _index = 0;
            if (_index > 48) _index = 48;

            return code;



            //byte code;
            //short diff, E, SS;

            //SS = kStepSize[_index];
            //code = 0x00;
            //if ((diff = (short)(sample - _last)) < 0)
            //{
            //    code = 0x08;
            //}
            //E = diff < 0 ? (short)-diff : diff;
            //if (E >= SS)
            //{
            //    code = (byte)(code | 0x04);
            //    E -= SS;
            //}
            //if (E >= SS / 2)
            //{
            //    code = (byte)(code | 0x02);
            //    E -= (short)(SS / 2);
            //}
            //if (E >= SS / 4)
            //{
            //    code = (byte)(code | 0x01);
            //}
            ///*    stat->step_index += step_adjust( code );
            //    if( stat->step_index < 0 ) stat->step_index = 0;
            //    if( stat->step_index > 48 ) stat->step_index = 48;
            //*/
            ///*
            //* Use the decoder to set the estimate of last sample.
            //* It also will adjust the step_index for us.
            //*/
            //_last = Decode(code);
            //return code;

        }

        public byte[] DecodeToLinearPCM()
        {
            List<byte> output = new List<byte>();

            for (int adpcmReadIndex = 0; adpcmReadIndex < _data.Length; ++adpcmReadIndex)
            {
                byte sourceAdpcmByte = _data[adpcmReadIndex];
                short decoded = (short)(kVolume * Decode((byte)((sourceAdpcmByte >> 4) & 0xf)));
                output.Add((byte)(decoded & 0xff));
                output.Add((byte)(decoded >> 8));

                decoded = (short)(kVolume * Decode((byte)(sourceAdpcmByte & 0xf)));
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
