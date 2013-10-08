using System;
using System.Windows.Media;

namespace CJToolkit.Util
{
    public static class ColorExtention
    {
        public static byte[] GetColorByte(this Color color)
        {
            return new[]{color.A,color.R,color.G,color.B};
        }

        public static byte[] GetColorByte(this int color)
        {
            var bytes= BitConverter.GetBytes(color);
            var array = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                array[i] = bytes[bytes.Length - i - 1];
            }
            return array;
        }

        public static int GetColorInteger(this Color color)
        {
            return color.GetColorByte().GetColorInteger();
        }

        public static int GetColorInteger(this byte[] color)
        {
            var array=new byte[color.Length];
            for (int i = 0; i<color.Length; i++)
            {
                array[i] = color[color.Length - i - 1];
            }
            return BitConverter.ToInt32(array, 0);
        }

        public static Color GetColor(this int color)
        {
            return color.GetColorByte().GetColor();
        }

        public static Color GetColor(this byte[] color)
        {
            if(color.Length!=4&&color.Length!=3) throw new ArgumentOutOfRangeException("color","A color byte array should have a lenth of 4 or 3");
            var array=new byte[4];
            if (color.Length == 3)
            {
                color.CopyTo(array,1 );
                array[0] = 0xff;
            }
            else
            {
                color.CopyTo(array,0);
            }
            return Color.FromArgb(array[0],array[1],array[2],array[3]);
        }

    }

}
