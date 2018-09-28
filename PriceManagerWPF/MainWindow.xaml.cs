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

        public static List<ModelData> Models = new List<ModelData>();
        public static int id = -1;

        public static SolidColorBrush empty = new SolidColorBrush(Colors.MediumVioletRed);

        public static ScriptManager manager;
        [ComVisible(true)] //Visible to the js
        public class ScriptManager
        {
            private Window[] windows;

            public ScriptManager(Window[] _windows)
            {
                windows = _windows;
            }

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

        public MainViewModel viewModel = new MainViewModel();

        public MainWindow()
        {
            Hub.mainWindow = this;
            InitializeComponent();
            Hub.window_colorpicker = new Window_colorpicker();
            Hub.window_material = new Window_material();
            Hub.window_model = new Window_model();

            Hub.window_model.Show();
            Hub.window_material.Show();

            manager = new ScriptManager(new Window[] {Hub.mainWindow, Hub.window_colorpicker, Hub.window_material, Hub.window_model });

            modelDataView.Visibility = Visibility.Hidden;
            SetBindings();

            var col = new ColorWpf().FromHsv(0, 1, 1);
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
                Hub.window_material.list_Materials.Items.Add(label);

                Material m = new Material();
                m.Name = (string)label.Content;
                model.Materials[i] = m;
            }

        }

        private void DisplayMaterials(Material[] materials)
        {

            if (materials != null && materials.Length > 0)
            {
                //ToggleBrowser(Visibility.Visible);
                for (int i = 0; i < materials.Length; i++)
                {
                    var label = new Label();
                    label.Content = materials[i].Name;
                    Hub.window_material.list_Materials.Items.Add(label);
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

            list_Models.Items.Add(label);

            viewModel.Item = model;

            //if (listView.Items.Count > 0)
            //{
            //    listView.SelectedIndex = listView.Items.Count - 1;

            //    Keyboard.Focus(listView.SelectedItem as ListViewItem);
            //}

        }

    
        private void comboBoxPriceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void imageButton_Click(object sender, RoutedEventArgs e)
        {

           
        }
        
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void saveItems(object sender, RoutedEventArgs e)
        {

            Controller.GenerateRequest("Home", "SavePriceList", "POST", Models);

        }

        private void window_Unloaded(object sender, RoutedEventArgs e)
        {
            Cef.Shutdown();
        }

        private void UpdateBindings()
        {

            DataContext = null;

            Hub.window_material.list_Materials.Items.Clear();
            DataContext = viewModel;

            Hub.window_model.DataContext = null;
            Hub.window_model.DataContext = viewModel;

            Hub.window_material.DataContext = null;
            Hub.window_material.DataContext = DataContext;

            Hub.window_colorpicker.DataContext = null;
            Hub.window_colorpicker.DataContext = DataContext;


            foreach (var mat in viewModel.Item.Materials)
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

        private void list_Models_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (list_Models.SelectedIndex > -1)
            {
                viewModel.Item = Models[list_Models.SelectedIndex];

                UpdateBindings();
                modelDataView.Visibility = Visibility.Visible;
            }
        }

        private void button_AddModel(object sender, RoutedEventArgs e)
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

                    Hub.window_model.chromeBrowser.ExecuteScriptAsync("loadModel", model.ModelJsonData, JsonConvert.SerializeObject(model.Materials));

                }
                catch (Exception)
                {

                    n = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                }

            }

            AddModel(model);
        }

        private void setType(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (sender as TextBox);

            if (txt.Text.ToLower().Contains("truss"))
            {
                Hub.window_model.truss_Container.Visibility = Visibility.Visible;
                Hub.window_model.items_Container.Visibility = Visibility.Hidden;

            }
            else if (txt.Text.ToLower().Contains("window") || txt.Text.ToLower().Contains("gate") || txt.Text.ToLower().Contains("door"))
            {
                Hub.window_model.truss_Container.Visibility = Visibility.Hidden;
                Hub.window_model.items_Container.Visibility = Visibility.Visible;
            }

        }
    }
}
