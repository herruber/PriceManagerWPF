using CefSharp;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for Window_material.xaml
    /// </summary>
    public partial class Window_material : Window
    {



        public Window_material()
        {
            Hub.window_material = this;
            InitializeComponent();
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
            Hub.window_model.chromeBrowser.ExecuteScriptAsync("setTexture", base64, property.Name, Hub.mainWindow.viewModel.material.tiling[0], Hub.mainWindow.viewModel.material.tiling[1]);
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

        private static string colorType = "";

        private void diffuseColor(object sender, RoutedEventArgs e)
        {
            Hub.window_colorpicker.Show();
            Button btn = (sender as Button);
            colorType = "color";
        }

        private void setNormalScale(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (sender as Slider);

            if (IsLoaded)
            {
                Hub.window_model.chromeBrowser.ExecuteScriptAsync("setNormalScale", slider.Value, slider.Value);
            }
        }

        private void setRoughness(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (sender as Slider);

            if (IsLoaded)
            {
                Hub.window_model.chromeBrowser.ExecuteScriptAsync("setRoughness", slider.Value);
            }
        }

        private void setDisplacement(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (sender as Slider);

            if (Hub.window_model.chromeBrowser != null)
            {
                Hub.window_model.chromeBrowser.ExecuteScriptAsync("setDisplacement", slider.Value);
            }
        }

        private void setMetalness(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (sender as Slider);

            if (IsLoaded)
            {
                Hub.window_model.chromeBrowser.ExecuteScriptAsync("setMetalness", slider.Value);
            }
        }

        private void UpdateBindings()
        {

            DataContext = null;

            Hub.window_material.list_Materials.Items.Clear();
            DataContext = Hub.mainWindow.viewModel;


            foreach (var mat in Hub.mainWindow.viewModel.Item.Materials)
            {

                Label label = new Label();
                label.Content = mat.Name;

                Binding binding = new Binding();
                binding.Path = new PropertyPath("Name");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Source = mat;

                Hub.window_material.list_Materials.Items.Add(label);
            }


        }

        private void list_Materials_Changed(object sender, SelectionChangedEventArgs e)
        {

           Hub.mainWindow.viewModel.materialId = list_Materials.SelectedIndex;

            if (Hub.mainWindow.viewModel.materialId > -1)
            {
                Hub.window_model.chromeBrowser.ExecuteScriptAsync("selectMaterial", Hub.mainWindow.viewModel.materialId);
                Hub.mainWindow.viewModel.material = Hub.mainWindow.viewModel.Item.Materials[Hub.mainWindow.viewModel.materialId];
                UpdateBindings();

                SolidColorBrush b = new SolidColorBrush();
                b.Color = Colors.OrangeRed;

                Hub.window_material.map.Background = b;
                Hub.window_material.normalMap.Background = b;
                Hub.window_material.roughnessMap.Background = b;
                Hub.window_material.displacementMap.Background = b;
                Hub.window_material.metalnessMap.Background = b;


                Hub.window_material.Show();
            }
            else
            {
                Hub.window_material.Hide();
            }
        

    }
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
}
