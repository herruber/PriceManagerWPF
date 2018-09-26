using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using CefSharp;
using CefSharp.Wpf;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Windows.Data;

namespace PriceManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static ChromiumWebBrowser chromeBrowser;
        public static List<ModelData> Models = new List<ModelData>();

        //public ModelData item { get; set; }

        public static int id = -1;

        public static SolidColorBrush empty = new SolidColorBrush(Colors.MediumVioletRed);

        public static ScriptManager manager;
        [ComVisible(true)] //Visible to the js
        public class ScriptManager
        {
            private Window window;

            public ScriptManager(Window _window)
            {
                window = _window;
            }

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

        private void SetBindings()
        {
        //    public string Name { get; set; }

        ////Base64 strings
        //public string map { get; set; }
        //public string normalMap { get; set; }
        //public string roughnessMap { get; set; }
        //public string displacementMap { get; set; }

        //public float[] tiling { get; set; }
        //public float roughness { get; set; }
        //public float metalness { get; set; }
        //public float normalScale { get; set; }
        //public float displacementScale { get; set; }


        //Bindings.Add(typeof(ModelData).GetProperty("Name"), textBox_base_Name);
        //    Bindings.Add(typeof(ModelData).GetProperty("Type"), textBox_base_Type);

        //    Bindings.Add(typeof(SizeData).GetProperty("AngleDeg"), textBox_size_AngleDeg);
        //    Bindings.Add(typeof(SizeData).GetProperty("Width"), textBox_size_Width);
        //    Bindings.Add(typeof(SizeData).GetProperty("Height"), textBox_size_Height);
        //    Bindings.Add(typeof(SizeData).GetProperty("Depth"), textBox_size_Depth);

        //    Bindings.Add(typeof(PriceData).GetProperty("Discount"), textBox_price_Discount);
        //    Bindings.Add(typeof(PriceData).GetProperty("Price"), textBox_price_Price);

        //    //Bindings.Add(typeof(Material).GetProperty("Discount"), textbox);
        }

        public static MainViewModel viewModel;

        public MainWindow()
        {

            InitializeComponent();
            groupBox.Visibility = Visibility.Hidden;
            materialDisplay.Visibility = Visibility.Hidden;
            lightControls.Visibility = Visibility.Hidden;
            browserContent.Visibility = Visibility.Hidden;
            palette.Visibility = Visibility.Hidden;

            InitializeChromium();
            SetBindings();

            var col = new ColorWpf().FromHsv(0, 1, 1);
            viewModel = new MainViewModel();
            SetSelectedColor(col);
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

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }


        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();

            settings.RemoteDebuggingPort = 8088;
            BrowserSettings sett = new BrowserSettings();
            sett.Javascript = CefState.Enabled;

            sett.WebSecurity = CefState.Disabled;
            // Initialize cef with the provided settings
            Cef.Initialize(settings);

            // Create a browser component

            chromeBrowser = new ChromiumWebBrowser();

            // Add it to the form and fill it to the form window.

            string curDir = AppDomain.CurrentDomain.BaseDirectory;

            chromeBrowser.Address = string.Format("file:///{0}/Web/scene.html", curDir);

            browserContent.Content = chromeBrowser;

        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MaterialsFromFile(ThreeJsonMaterial[] materials, ModelData model)
        {

            model.Materials = new Material[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                var label = new Label();
                label.Content = materials[i].DbgName;
                listViewMaterials.Items.Add(label);

                Material m = new Material();
                m.Name = (string)label.Content;
                model.Materials[i] = m;
            }

        }

        private void ToggleBrowser(Visibility vis)
        {
                browserContent.Visibility = vis;
                lightControls.Visibility = vis;

        }

        private void DisplayMaterials(Material[] materials)
        {
            ToggleBrowser(Visibility.Hidden);
            if (materials != null && materials.Length > 0)
            {
                //ToggleBrowser(Visibility.Visible);
                for (int i = 0; i < materials.Length; i++)
                {
                    var label = new Label();
                    label.Content = materials[i].Name;
                    listViewMaterials.Items.Add(label);
                }
            }
        }

        public void PresentModelForm(ModelData item)
        {

            //textBox_base_Name.Text = item.Name;
            //textBox_base_Type.Text = item.Type;

            //textBox_size_AngleDeg.Text = item.SizeData.AngleDeg.ToString();
            //textBox_size_Depth.Text = item.SizeData.Depth.ToString();
            //textBox_size_Width.Text = item.SizeData.Width.ToString();
            //textBox_size_Height.Text = item.SizeData.Height.ToString();

            //listViewMaterials.Items.Clear();

            //DisplayMaterials(item.Materials);

            //switch (item.PriceData.Pricetype)
            //{
            //    case PriceData.PriceType.Sqm:
            //        comboBoxPriceType.SelectedIndex = 2;
            //        break;
            //    case PriceData.PriceType.M:
            //        comboBoxPriceType.SelectedIndex = 1;
            //        break;
            //    case PriceData.PriceType.Unit:
            //        comboBoxPriceType.SelectedIndex = 0;
            //        break;
            //    default:
            //        break;
            //}
        }

        private void controlTextChange(object sender, RoutedEventArgs e)
        {

        }

        WrapPanel AddProperty(string name, string targetProperty, Control control)
        {
            var w = new WrapPanel();

            var t = new TextBlock();

            t.Margin = new Thickness(0, 0, 10, 0);
            t.Text = name;

            control.MinWidth = 100;

            w.Children.Add(t);
            w.Children.Add(control);
            control.Name = "control_" + targetProperty;

            if (control.GetType() == typeof(TextBox))
            {
                (control as TextBox).TextChanged += controlTextChange;
            }

            return w;
        }

        public void AddModel(ModelData model)
        {
            Models.Add(model);
            var label = new Label();

            Binding binding = new Binding();
            binding.Path = new PropertyPath("Name");
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = model;

            label.SetBinding(ContentProperty, binding);

            label.Content = model.Name;

            listView.Items.Add(label);

            viewModel.Item = model;

            if (listView.Items.Count > 0)
            {
                listView.SelectedIndex = listView.Items.Count - 1;

                Keyboard.Focus(listView.SelectedItem as ListViewItem);
            }

        }

        private void UpdateBindings()
        {

            DataContext = null;

            listViewMaterials.Items.Clear();
            DataContext = viewModel;


            foreach (var mat in viewModel.Item.Materials)
            {

                Label label = new Label();
                label.Content = mat.Name;

                Binding binding = new Binding();
                binding.Path = new PropertyPath("Name");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Source = mat;

                listViewMaterials.Items.Add(label);
            }


        }


        //Modeldata selection changed
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            id = listView.SelectedIndex;

            if (viewModel.Item != null)
            {
                viewModel.Item = Models[id];

                UpdateBindings();

                //PresentModelForm(item);

                groupBox.Visibility = Visibility.Visible;
                listBox.Visibility = Visibility.Visible;
            }
            else
            {
                groupBox.Visibility = Visibility.Hidden;
            }

        }

        //Add json 3d model
        private void button1_Click(object sender, RoutedEventArgs e)
        {

            ModelData model = new ModelData();
            model.Name = "New Model";
            model.Type = "none";

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Json files (*.json)|*.json";

            if (dlg.ShowDialog() == true)
            {

                string input = File.ReadAllText(dlg.FileName);

                listBox.Visibility = Visibility.Visible;

                string n = "";
                try
                {

                    model.Name = Path.GetFileNameWithoutExtension(dlg.FileName);
                    model.ModelJsonData = input;
                    model.ModelFileName = Path.GetFileName(dlg.FileName);

                    ThreeJsonModel modelB = JsonConvert.DeserializeObject<ThreeJsonModel>(input);
                    MaterialsFromFile(modelB.materials, model);

                    model.JsonModel = modelB;


                }
                catch (Exception)
                {

                    n = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                }

            }

            AddModel(model);
        }

        private void comboBoxPriceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void imageButton_Click(object sender, RoutedEventArgs e)
        {

           
        }

        private void floatProperty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (sender as Slider);


            if (chromeBrowser != null)
            {

                if (slider.Name != "rotation")
                {
                    PropertyInfo property = typeof(Material).GetProperty(slider.Name);
                    Type type = property.GetType();


                    property.SetValue(viewModel.material, (float)slider.Value);
                    chromeBrowser.ExecuteScriptAsync("setFloat", slider.Value, property.Name);


                }
                else
                {
                    chromeBrowser.ExecuteScriptAsync("setFloat", slider.Value, "rotation");
                }
            }
        }

        private void materialList_Changed(object sender, SelectionChangedEventArgs e)
        {

            viewModel.materialId = listViewMaterials.SelectedIndex;

            if (viewModel.materialId > -1)
            {
                viewModel.material = viewModel.Item.Materials[viewModel.materialId];
                UpdateBindings();

                SolidColorBrush b = new SolidColorBrush();
                b.Color = Colors.OrangeRed;

                map.Background = b;
                normalMap.Background = b;
                roughnessMap.Background = b;
                displacementMap.Background = b;
                metalnessMap.Background = b;

                chromeBrowser.ExecuteScriptAsync("setMaterial", JsonConvert.SerializeObject(viewModel.material));
                materialDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                materialDisplay.Visibility = Visibility.Hidden;
            }

            if (viewModel.material != null)
            {
                lightControls.Visibility = Visibility.Visible;
                browserContent.Visibility = Visibility.Visible;
            }
            else
            {
                lightControls.Visibility = Visibility.Hidden;
                browserContent.Visibility = Visibility.Hidden;
            }
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
            Debug.WriteLine("x: " +p.X + " y:" + p.Y);
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
                typeof(Material).GetProperty(colorType).SetValue(viewModel.material, colf);

                chromeBrowser.ExecuteScriptAsync("setColor4", JsonConvert.SerializeObject(colf), colorType);

                SolidColorBrush b = new SolidColorBrush();
                b.Color = c;

                color_button.Background = b;
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

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private List<TextBox> GetHSV()
        {


            TextBox bright = null;
            TextBox sat = null;
            TextBox hue = null;

            TextBox red = null;
            TextBox green = null;
            TextBox blue = null;

            foreach (object item in colorStackPanel.Children)
            {

                var j = item as Canvas;


                foreach (var child in j.Children)
                {
                    var i = child as TextBox;
                    if (i != null)
                    {
                        if (i.Name == "text_bright")
                        {
                            bright = i;
                        }
                        else if (i.Name == "text_sat")
                        {
                            sat = i;
                        }
                        else if (i.Name == "text_hue")
                        {
                            sat = i;
                        }
                        else if (i.Name == "text_red")
                        {
                            red = i;
                        }
                        else if (i.Name == "text_green")
                        {
                            green = i;
                        }
                        else if (i.Name == "text_blue")
                        {
                            blue = i;
                        }
                    }
                }
            }

            return new List<TextBox>() { hue, sat, bright, red, green, blue };
        }

        private void colorText_Changed(object sender, TextChangedEventArgs e)
        {

            var input = (sender as TextBox);

            if (!string.IsNullOrEmpty(input.Text) && this.IsLoaded)
            {
                string type = input.Name.Split('_')[1];

                List<TextBox> fields = GetHSV();
                ColorWpf col = null;


                if (type.ToLower() == "red" || type.ToLower() == "green" || type.ToLower() == "blue")
                {

                    col = new ColorWpf().FromHsv(float.Parse(fields[0].Text), float.Parse(fields[1].Text) / 100f, float.Parse(fields[2].Text) / 100f);
                    SetSelectedColor(col);
                }
                else
                {
                    col = new ColorWpf().FromHsv(float.Parse(fields[0].Text), float.Parse(fields[1].Text) / 100f, float.Parse(fields[2].Text) / 100f);
                    SetSelectedColor(col);
                }
            }
        }

        private void saveItems(object sender, RoutedEventArgs e)
        {

            Controller.GenerateRequest("Home", "SavePriceList", "POST", Models);

        }

        private void window_Unloaded(object sender, RoutedEventArgs e)
        {
            Cef.Shutdown();
        }

        private string OpenFile(string type = null)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == true)
            {


                return dlg.FileName;
            }

            return null;
        }

        private ImageBrush LoadImage(string file)
        {
            ImageSource imageSource = new BitmapImage(new Uri(file));

            ImageBrush b = new ImageBrush();
            b.ImageSource = imageSource;
            return b;
        }

        private void SetTexture(PropertyInfo property, Button btn)
        {

            string file = OpenFile();

            btn.Background = LoadImage(file);

            var base64 = Convert.ToBase64String(File.ReadAllBytes(file));
            chromeBrowser.ExecuteScriptAsync("setTexture", base64, property.Name, viewModel.material.tiling[0], viewModel.material.tiling[1]);
        }

        private void mapClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);

            SetTexture(typeof(Material).GetProperty("map"), btn);

        }

        private void normalMapClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);

            SetTexture(typeof(Material).GetProperty("normalMap"), btn);
        }

        private void roughnessMapClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);

            SetTexture(typeof(Material).GetProperty("roughnessMap"), btn);
        }

        private void displacementMapClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);

            SetTexture(typeof(Material).GetProperty("displacementMap"), btn);
        }

        private void metalnessMapClick(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);

            SetTexture(typeof(Material).GetProperty("metalnessMap"), btn);
        }

        public class NameValuePair
        {
            public string name { get; set; }
            public object value { get; set; }

            public NameValuePair(string _name, object _value)
            {
                name = _name;
                value = _value;
            }
        }

        NameValuePair[] pairs = new NameValuePair[] {
                new NameValuePair("roughness", null),
                new NameValuePair("tiling", null),
                new NameValuePair("metalness", null),
                new NameValuePair("normalScale", null)
            };

        private void materialTextChanged(object sender, TextChangedEventArgs e)
        {
            if (viewModel != null)
            {
                pairs[0].value = viewModel.material.roughness;
                pairs[1].value = viewModel.material.tiling;
                pairs[2].value = viewModel.material.metalness;
                pairs[3].value = viewModel.material.normalScale;


                chromeBrowser.ExecuteScriptAsync("updateMaterial", JsonConvert.SerializeObject(pairs));
            }

        }

        private void materialDoubleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewModel != null)
            {
                pairs[0].value = viewModel.material.roughness;
                pairs[1].value = viewModel.material.tiling;
                pairs[2].value = viewModel.material.metalness;
                pairs[3].value = viewModel.material.normalScale;


                chromeBrowser.ExecuteScriptAsync("updateMaterial", JsonConvert.SerializeObject(pairs));
            }
        }

        private static string colorType = "";

        private void diffuseColor(object sender, RoutedEventArgs e)
        {
            palette.Visibility = Visibility.Visible;

            Button btn = (sender as Button);
            colorType = "color";
        }

        private void pickColor(object sender, RoutedEventArgs e)
        {
            palette.Visibility = Visibility.Hidden;

        }
    }
}
