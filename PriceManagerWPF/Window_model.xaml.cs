using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window_model.xaml
    /// </summary>
    public partial class Window_model : Window
    {

        public ChromiumWebBrowser chromeBrowser;

        public Window_model()
        {

            Hub.window_model = this;
            InitializeComponent();
            InitializeChromium();
        }

        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();

            settings.RemoteDebuggingPort = 8088;
            BrowserSettings sett = new BrowserSettings();
            sett.Javascript = CefState.Enabled;
            sett.FileAccessFromFileUrls = CefState.Enabled;
            sett.UniversalAccessFromFileUrls = CefState.Enabled;
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

        private void setLightRotation(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (sender as Slider);

            if (Hub.window_model.chromeBrowser != null)
            {
                Hub.window_model.chromeBrowser.ExecuteScriptAsync("setLightRotation", slider.Value);
            }
        }

        public float pivotX = 0;
        public float pivotY = 0;
        public float pivotZ = 0;

        private void pivotBtn_Click(object sender, RoutedEventArgs e)
        {
            chromeBrowser.ExecuteScriptAsync("updatePivot", 0);
        }

        private void pivot_Changed(object sender, TextChangedEventArgs e)
        {

            TextBox box = (sender as TextBox);

            string[] info = box.Name.Split('_');
            float val = float.Parse(box.Text);

            switch (info[1])
            {
                case "x":
                    pivotX = val;
                    break;
                case "y":
                    pivotY = val;
                    break;
                case "z":
                    pivotZ = val;
                    break;
                default:
                    break;
            }

        }
    }
}
