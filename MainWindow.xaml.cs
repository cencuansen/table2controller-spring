using Microsoft.Web.WebView2.Core;
using net6_wpf_webview2_template.HostObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace net6_wpf_webview2_template
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      

        public MainWindow()
        {
            InitializeComponent();
            _ = InitializeAsync();
            
        }

        private async Task InitializeAsync()
        {
            var options = new CoreWebView2EnvironmentOptions("--disable-web-security");
            var environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await WebView2.EnsureCoreWebView2Async(environment);
            // WebView2.CoreWebView2.SetVirtualHostNameToFolderMapping("app", "vue-app/dist", CoreWebView2HostResourceAccessKind.Allow);
            // WebView2.CoreWebView2.Navigate("http://app/index.html");
            WebView2.CoreWebView2.Navigate("http://localhost:5173");

            WebView2.CoreWebView2.AddHostObjectToScript("hostObjectOne", new HostObjectOne());
        }
    }
}
