using System.Windows;
using System.Windows.Controls;

namespace DxMVCRichEdit_OpenSilver_Bridge
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await OpenSilver.Interop.LoadCssFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/dx.light.css");
            await OpenSilver.Interop.LoadCssFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/dx.richedit.css");

            await OpenSilver.Interop.LoadJavaScriptFile("https://code.jquery.com/jquery-3.5.1.min.js");
            await OpenSilver.Interop.LoadJavaScriptFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/jszip.min.js");
            await OpenSilver.Interop.LoadJavaScriptFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/dx.all.js");
            await OpenSilver.Interop.LoadJavaScriptFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/dx.richedit.min.js");
            await OpenSilver.Interop.LoadJavaScriptFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/nspell.js");
            await OpenSilver.Interop.LoadJavaScriptFile("ms-appx:///DxMVCRichEdit_OpenSilver_Bridge/js/richedit-creator.js");

            var initScript = @"createRichEdit('api/SaveDocument');";
            OpenSilver.Interop.ExecuteJavaScript(initScript);
        }
    }
}