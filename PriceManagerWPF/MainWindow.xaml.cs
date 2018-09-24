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

namespace PriceManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool isReady = false;
        public static ChromiumWebBrowser chromeBrowser;
        public static List<ModelData> Models = new List<ModelData>();
        public static ModelData item = null;
        public static Material material = null;
        public static int id = -1;
        public static int materialId = -1;
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

        public MainWindow()
        {
            InitializeComponent();
            groupBox.Visibility = Visibility.Hidden;
            materialDisplay.Visibility = Visibility.Hidden;
            lightControls.Visibility = Visibility.Hidden;
            browserContent.Visibility = Visibility.Hidden;
            palette.Visibility = Visibility.Hidden;

            this.DataContext = item;

            InitializeChromium();

            var col = new ColorWpf().FromHsv(0, 1, 1);
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

        private void PopulateItems()
        {



        }

        public static List<Slider> materialSliders = new List<Slider>();
        public static List<Button> materialMaps = new List<Button>();

        private void LoadPriceData()
        {
            string data = Controller.GenerateRequest("Home", "getPriceList", "GET", null);

            List<ModelData> modeldata = JsonConvert.DeserializeObject<List<ModelData>>(data);


            foreach (var model in modeldata)
            {

                textBox_base_Name.Text = model.Name;
                AddModel(model);
            }

            listBox.Visibility = Visibility.Visible;

        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock tb in FindVisualChildren<TextBlock>(mainWindow))
            {

                SolidColorBrush brush = new SolidColorBrush();
                var col = new Color();

                col.R = 154;
                col.G = 255;
                col.B = 186;
                brush.Color = col;

                tb.Background = brush;
            }

            foreach (Slider slider in FindVisualChildren<Slider>(mainWindow))
            {

                //If material slider
                if (materialPanel.Children.IndexOf((slider.Parent as Canvas)) > -1)
                {
                    materialSliders.Add(slider);
                }

            }

            foreach (Button btn in FindVisualChildren<Button>(mainWindow))
            {

                //If material slider
                if (materialPanel.Children.IndexOf((btn.Parent as Canvas)) > -1)
                {
                    materialMaps.Add(btn);
                }

            }

            LoadPriceData();
            isReady = true;
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MaterialsFromFile(ThreeJsonMaterial[] materials)
        {
            item.Materials = new Material[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                var label = new Label();
                label.Content = materials[i].DbgName;
                listViewMaterials.Items.Add(label);

                Material m = new Material();
                m.Name = (string)label.Content;
                item.Materials[i] = m;
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

            textBox_base_Name.Text = item.Name;
            textBox_base_Type.Text = item.Type;

            textBox_size_AngleDeg.Text = item.SizeData.AngleDeg.ToString();
            textBox_size_Depth.Text = item.SizeData.Depth.ToString();
            textBox_size_Width.Text = item.SizeData.Width.ToString();
            textBox_size_Height.Text = item.SizeData.Height.ToString();

            listViewMaterials.Items.Clear();

            DisplayMaterials(item.Materials);

            switch (item.PriceData.Pricetype)
            {
                case PriceData.PriceType.Sqm:
                    comboBoxPriceType.SelectedIndex = 2;
                    break;
                case PriceData.PriceType.M:
                    comboBoxPriceType.SelectedIndex = 1;
                    break;
                case PriceData.PriceType.Unit:
                    comboBoxPriceType.SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }

        private void controlTextChange(object sender, RoutedEventArgs e)
        {

            if (isReady)
            {
                ModelData model = item;
                SizeData size = model.SizeData;

                var t = (sender as TextBox);
                string info = t.Name;
                string propertyName = info.Split('_').Last();
                string category = info.Split('_')[1];
                string val = t.Text;

                switch (category.ToLower())
                {
                    case "base":
                        typeof(ModelData).GetProperty(propertyName).SetValue(model, val);

                        if (propertyName == "Name")
                        {
                            (listView.SelectedItem as Label).Content = val;

                        }
                        break;
                    case "size":
                        PropertyInfo prop = typeof(SizeData).GetProperty(propertyName);
                        Type type = prop.PropertyType;

                        if (val == null || val == "")
                        {
                            return;
                        }

                        if (type == typeof(int))
                        {
                            prop.SetValue(size, int.Parse(val));
                        }
                        else if (type == typeof(float))
                        {
                            prop.SetValue(size, float.Parse(val));
                        }

                        else
                        {
                            prop.SetValue(size, val);
                        }


                        break;
                    case "material":
                        typeof(Material).GetProperty(propertyName).SetValue(model, val);
                        break;
                    default:
                        break;
                }
            }
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
            label.Content = model.Name;
            listView.Items.Add(label);

            if (listView.Items.Count > 0)
            {
                listView.SelectedIndex = listView.Items.Count - 1;
                item = Models[listView.SelectedIndex];
                Keyboard.Focus(listView.SelectedItem as ListViewItem);
            }

        }


        //Modeldata selection changed
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            id = listView.SelectedIndex;
            item = Models[id];


            if (item != null)
            {
                PresentModelForm(item);

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

            AddModel(model);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Json files (*.json)|*.json";

            if (dlg.ShowDialog() == true)
            {

                string input = File.ReadAllText(dlg.FileName);


                listBox.Visibility = Visibility.Visible;
                string n = "";
                try
                {

                    item.Name = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                    (listView.SelectedItem as Label).Content = item.Name;
                    n = item.Name;
                    item.ModelJsonData = input;
                    item.ModelFileName = Path.GetFileName(dlg.FileName);

                    ThreeJsonModel modelB = JsonConvert.DeserializeObject<ThreeJsonModel>(input);
                    MaterialsFromFile(modelB.materials);

                    item.JsonModel = modelB;


                }
                catch (Exception)
                {

                    n = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                }

                textBox_base_Name.Text = n;
            }
        }

        private void comboBoxPriceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (comboBoxPriceType.SelectedIndex)
            {
                case 0:
                    item.PriceData.Pricetype = PriceData.PriceType.Unit;
                    break;
                case 1:
                    item.PriceData.Pricetype = PriceData.PriceType.M;
                    break;
                case 2:
                    item.PriceData.Pricetype = PriceData.PriceType.Sqm;
                    break;
                default:
                    break;
            }

        }

        private void imageButton_Click(object sender, RoutedEventArgs e)
        {

            Button btn = (sender as Button);
            Canvas canvas = (Canvas)btn.Parent;
            string mapType = "";

            switch (canvas.Name)
            {
                case "groupMap":
                    mapType = "map";
                    break;
                case "groupDisplacement":
                    mapType = "displacementMap";
                    break;
                case "groupRoughness":
                    mapType = "roughnessMap";
                    break;
                case "groupNormal":
                    mapType = "normalMap";
                    break;
                default:
                    break;
            }

            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == true)
            {


                PropertyInfo property = typeof(Material).GetProperty(mapType);

                ImageSource imageSource = new BitmapImage(new Uri(dlg.FileName));

                ImageBrush b = new ImageBrush();
                b.ImageSource = imageSource;

                btn.Background = b;


                byte[] data;

                var path = dlg.FileName;

                var base64 = Convert.ToBase64String(File.ReadAllBytes(dlg.FileName));

                property.SetValue(material, base64);

                chromeBrowser.ExecuteScriptAsync("setTexture", base64, mapType);
            }

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


                    property.SetValue(material, (float)slider.Value);
                    chromeBrowser.ExecuteScriptAsync("setFloat", slider.Value, property.Name);


                }
                else
                {
                    chromeBrowser.ExecuteScriptAsync("setFloat", slider.Value, "rotation");
                }
            }
        }

        private void PresentMaterial()
        {
            foreach (var slider in materialSliders)
            {

                var prop = typeof(Material).GetProperty(slider.Name);

                if (prop != null)
                {

                    double value = 0;
                    double.TryParse(prop.GetValue(material).ToString(), out value);
                    slider.Value = value;
                }

            }

            foreach (var map in materialMaps)
            {
                var prop = typeof(Material).GetProperty(map.Name);

                if (prop != null)
                {

                    var value = prop.GetValue(material);

                    if (value == null)
                    {
                        map.Background = empty;
                    }
                    else
                    {

                        byte[] binaryData = Convert.FromBase64String(value.ToString());

                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(binaryData);
                        bi.EndInit();

                        ImageBrush brush = new ImageBrush(bi);


                        map.Background = brush;
                    }
                }
            }
        }

        private void materialList_Changed(object sender, SelectionChangedEventArgs e)
        {

            materialId = listViewMaterials.SelectedIndex;

            if (materialId > -1)
            {
                material = item.Materials[materialId];
                PresentMaterial();
                chromeBrowser.ExecuteScriptAsync("setMaterial", JsonConvert.SerializeObject(material));
                materialDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                materialDisplay.Visibility = Visibility.Hidden;
            }

            if (material != null)
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

        private void SampleXY(Point p, Image img)
        {

            ColorWpf col = GetPixelColor(p);

            double wx = p.X - 129;
            double wy = -(p.Y - 129);

            var dist = (float)Math.Sqrt(Math.Pow(wx, 2) + Math.Pow(wy, 2));

            if (dist <= 128)
            {

                SetSelectedColor(col);
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

            if (!string.IsNullOrEmpty(input.Text) && isReady)
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
    }
}
