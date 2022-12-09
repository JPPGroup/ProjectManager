using NativeBindings;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            webView.Source = new Uri("https://localhost:7028");
#else
            webView.Source = new Uri("http://services.cedarbarn.local/projectmanager/");
#endif
            webView.CoreWebView2InitializationCompleted += (e, o) =>
            {                
                webView.CoreWebView2.AddHostObjectToScript("projectdocuments", new ProjectDocuments());
            };

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        }
    }
}
