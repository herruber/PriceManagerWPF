using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceManagerWPF
{
    public static class ColorUtilities
    {

        public static float[] ToArray(ColorWpf color)
        {
            return new float[4] { Convert.ToSingle(color.R) / 255f, Convert.ToSingle(color.G) / 255f, Convert.ToSingle(color.B) / 255f, 1 };
        }

        public static ColorWpf ToColor(float[] color)
        {

            return new ColorWpf().FromRgb(Convert.ToByte(Math.Round(color[0] * 255)), Convert.ToByte(Math.Round(color[1] * 255)), Convert.ToByte(Math.Round(color[2] * 255)));
        }


    }

}
