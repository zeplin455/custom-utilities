using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZUtils.Utilities
{
    /// <summary>
    /// Helper class to convert to and from nibbles
    /// </summary>
    public static class KeyConvert
    {
        public static byte[] ConvertToNibbles(this string str)
        {
            byte[] initialBytes = Encoding.UTF8.GetBytes(str);
            return initialBytes.SplitNibbles();
        }

        public static string ConvertFromFromNibbles(this byte[] byteArr)
        {
            return Encoding.UTF8.GetString(byteArr.CombineNibbles());
        }

        public static byte[] SplitNibbles(this byte[] byteArr)
        {
            byte[] result = new byte[byteArr.Length * 2];

            for (int i = 0; i < byteArr.Length; ++i)
            {
                byte nibble1 = (byte)(byteArr[i] & 0x0F);
                byte nibble2 = (byte)((byteArr[i] & 0xF0) >> 4);
                result[i * 2] = nibble2;
                result[(i * 2) + 1] = nibble1;
            }

            return result;
        }

        public static byte[] SplitNibbles(this byte byteArr)
        {
            byte[] result = new byte[2];

            byte nibble1 = (byte)(byteArr & 0x0F);
            byte nibble2 = (byte)((byteArr & 0xF0) >> 4);
            result[0] = nibble2;
            result[1] = nibble1;

            return result;
        }

        public static byte[] CombineNibbles(this byte[] byteArr)
        {
            int length = byteArr.Length / 2;
            byte[] result = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                byte nibble1 = byteArr[(i * 2) + 1];
                byte nibble2 = byteArr[i * 2];

                result[i] = (byte)((nibble2 << 4) | nibble1);
            }
            return result;
        }
    }
}
