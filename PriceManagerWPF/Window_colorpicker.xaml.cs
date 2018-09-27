using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PriceManagerWPF
{
    /// <summary>
    /// Interaction logic for Window_colorpicker.xaml
    /// </summary>
    public partial class Window_colorpicker : Window
    {

        public Window_colorpicker()
        {
            Hub.window_colorpicker = this;
            InitializeComponent();
            CreatePalette();
        }

        public ColorWpf GetPixelColor(Point p)
        {
            int width = 257;
            double x = p.X;
            double y = p.Y;

            double wx = x - 129;
            double wy = -(y - 129);
            float max = (float)Math.Sqrt(Math.Pow(128, 2) + Math.Pow(128, 2));
            var id = x + y * width;
            id *= 3;

            var dist = (float)Math.Sqrt(Math.Pow(wx, 2) + Math.Pow(wy, 2));

            double angle = Math.Atan2(wy, wx);

            float deg = (float)(angle / Math.PI) * 180;

            if (deg < 0)
            {
                deg = 360 + deg;
            }

            ColorWpf color = new ColorWpf().FromHsv(deg, dist / max, 1.0);

            return color;

        }


        public void CreatePalette()
        {
            BitmapImage img = new BitmapImage();

            double dpi = 96;
            int width = 257;
            int height = 257;
            byte[] pixelData = new byte[width * height * 4];
            int stride = ((width * 24 + 23) & ~23) / 8;
            Random rand = new Random();

            Point p = new Point(129, 129);
            float max = 128;

            List<double> hues = new List<double>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double wx = x - 129;
                    double wy = -(y - 129);

                    var id = x + y * width;
                    id *= 4;

                    var dist = (float)Math.Sqrt(Math.Pow(wx, 2) + Math.Pow(wy, 2));
                    byte a = 255;

                    if (dist > 128)
                    {
                        a = 0;
                    }

                    double angle = Math.Atan2(wy, wx);

                    float deg = (float)(angle / Math.PI) * 180;

                    if (deg < 0)
                    {
                        deg = 360 + deg;
                    }

                    ColorWpf color = new ColorWpf().FromHsv(deg, dist / max, 1.0);

                    pixelData[id] = color.B;
                    pixelData[id + 1] = color.G;
                    pixelData[id + 2] = color.R;
                    pixelData[id + 3] = a;
                    hues.Add(deg);
                }
            }

            BitmapSource bmpSource = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Bgra32, null, pixelData, 257 * 4);

            image.Source = bmpSource;


        }

        private void SetSelectedColor(ColorWpf color)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color.GetMediaColor();
            selectedColor.Background = brush;
        }

        private void SampleXY(Point p, Image image)
        {

            ColorWpf col = GetPixelColor(p);
            Debug.WriteLine("x: " + p.X + " y:" + p.Y);
            BitmapSource img = (BitmapSource)image.Source;

            int stride = ((img.PixelWidth * 24 + 23) & ~23) / 8;

            float max = 128;

            int x = (int)p.X;
            int y = (int)p.Y;

            double wx = x - 129;
            double wy = -(y - 129);

            var id = x + y * img.PixelWidth;
            id *= 4;

            var dist = (float)Math.Sqrt(Math.Pow(wx, 2) + Math.Pow(wy, 2));

            double angle = Math.Atan2(wy, wx);

            float deg = (float)(angle / Math.PI) * 180;

            if (deg < 0)
            {
                deg = 360 + deg;
            }

            if (dist <= 128)
            {
                ColorWpf color = new ColorWpf().FromHsv(deg, dist / max, 1.0);
                SetSelectedColor(color);

                Color c = ((SolidColorBrush)selectedColor.Background).Color;
                float[] colf = new float[] { c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f };
                //typeof(Material).GetProperty(colorType).SetValue(Hub.mainWindow.viewModel.material, colf);

                Hub.window_model.chromeBrowser.ExecuteScriptAsync("setColor4", JsonConvert.SerializeObject(colf), "color");

                SolidColorBrush b = new SolidColorBrush();
                b.Color = c;

            }



        }

        private void samplePalette(object sender, MouseButtonEventArgs e)
        {
            Image img = (sender as Image);
            SampleXY(e.GetPosition(img), img);

        }

        private void paletteMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Image img = (sender as Image);
                SampleXY(e.GetPosition(img), img);
            }

        }


        private void pickColor(object sender, RoutedEventArgs e)
        {
            Hub.window_colorpicker.Hide();

        }
    }
}
