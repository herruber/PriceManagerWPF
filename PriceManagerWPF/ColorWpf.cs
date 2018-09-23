using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PriceManagerWPF
{

    public class RGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public RGB()
        {

        }

        public RGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public class ColorWpf
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Brightness { get; set; }

        public Color MediaColor { get; set; }

        public Color GetMediaColor()
        {
            MediaColor = Color.FromRgb((byte)R, (byte)G, (byte)B);

            return MediaColor;
        }

        public ColorWpf FromRgb(byte r, byte g, byte b)
        {
            return null;
        }

        public ColorWpf FromHsv(double h, double s, double v)
        {
            RGB result = HsvToRgb(h, s, v);
            Hue = h;
            Saturation = s;
            Brightness = v;

            R = result.R;
            G = result.G;
            B = result.B;

            return this;
        }

        RGB HsvToRgb(double h, double S, double V)
        {
            byte r;
            byte g;
            byte b;

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = (byte)Clamp((int)(R * 255.0));
            g = (byte)Clamp((int)(G * 255.0));
            b = (byte)Clamp((int)(B * 255.0));

            return new RGB(r, g, b);
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
