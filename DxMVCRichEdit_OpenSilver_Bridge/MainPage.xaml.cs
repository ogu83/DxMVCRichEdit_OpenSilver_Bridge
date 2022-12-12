using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DxMVCRichEdit_OpenSilver_Bridge
{

    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            arrangeExpanderItems();
            Loaded += MainPage_Loaded;
        }

        private void arrangeExpanderItems()
        {
            var e1 = AddExpander("Journal");
            AddMenuItem(e1, "Upmerksamheitinformation");

            var e2 = AddExpander("Avgifter");
            AddMenuItem(e2, "First Item");
            AddMenuItem(e2, "Second Item");
            AddMenuItem(e2, "Third Item");

            var e3 = AddExpander("HSL");
            AddMenuItem(e3, "First Item");
            AddMenuItem(e3, "Second Item");
            AddMenuItem(e3, "Third Item");

            var e4 = AddExpander("Halsorande");
            AddMenuItem(e4, "First Item");
            AddMenuItem(e4, "Second Item");
            AddMenuItem(e4, "Third Item");
            AddMenuItem(e4, "4th Item");
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


        private string GetMenuNameFromHyperlinkButton(HyperlinkButton button)
        {
            var sp = button.Content as StackPanel;
            if (sp != null)
            {
                var tb = sp.Children[0] as TextBlock;
                if (tb != null) return tb.Text;
            }

            return string.Empty;
        }

        private List<HyperlinkButton> SortButtonsByName(IEnumerable<HyperlinkButton> allButtons)
        {
            return allButtons.OrderBy(GetMenuNameFromHyperlinkButton).ToList();
        }

        public void AddMenuItem(System.Windows.Controls.Expander accordion, string menuHeader)
        {
            var buttonContent = new StackPanel { Orientation = Orientation.Horizontal };

            var header = new TextBlock
            {
                Text = menuHeader,
                Foreground = new SolidColorBrush(Colors.DarkGray),
                FontSize = 10.0,
                FontFamily = new FontFamily("Tahoma")
            };

            buttonContent.Children.Add(header);

            var menuItem = new HyperlinkButton
            {
                Style = Application.Current.Resources["AccordionHyperlinkButtonStyle"] as Style,
                Content = buttonContent,
                Margin = new Thickness(-3, 0, 0, 0),
            };

            var wp = (WrapPanel)((ScrollViewer)accordion.Content).Content;

            List<HyperlinkButton> allButtons = wp.Children.Cast<HyperlinkButton>().ToList();
            allButtons.Add(menuItem);

            allButtons = SortButtonsByName(allButtons);
            wp.Children.Clear();

            allButtons.ForEach(button => wp.Children.Add(button));

        }

        public System.Windows.Controls.Expander AddExpander(string header)
        {
            var sv = new ScrollViewer
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                BorderThickness = new Thickness(0),
                Content = new WrapPanel
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Orientation = Orientation.Vertical
                }
            };

            var buttonContent = new StackPanel { Orientation = Orientation.Horizontal };

            var tbHeader = new TextBlock { Text = header };
            buttonContent.Children.Add(tbHeader);

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.Orange),
                Height = 19,
                Width = 19,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(4, -3, 0, 0),
                Visibility = Visibility.Collapsed
            };

            buttonContent.Children.Add(border);

            var expanderItem = new System.Windows.Controls.Expander
            {
                Header = buttonContent,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = sv,
                Width = 170,
                Padding = new Thickness(0, 0, 0, 0)
            };

            expanderItem.Expanded += (s, e) =>
            {
                foreach (System.Windows.Controls.Expander expander in acContentMenu.Items.Where(q => q != expanderItem))
                {
                    expander.IsExpanded = false;
                    SetEnabledStatus(expander, true);
                }

                SetEnabledStatus(expanderItem, false);
            };

            expanderItem.Style = Application.Current.Resources["DefaultControlsToolkitExpanderMenuStyle"] as Style;

            if (acContentMenu.Items.Count == 0)
            {
                expanderItem.Loaded += OnExpanderLoaded;
                expanderItem.IsExpanded = true;
            }

            acContentMenu.Items.Add(expanderItem);

            return expanderItem;
        }

        private static void OnExpanderLoaded(object sender, EventArgs e)
        {
            var expanderItem = (System.Windows.Controls.Expander)sender;
            SetEnabledStatus(expanderItem, false);
            expanderItem.Loaded -= OnExpanderLoaded;
        }

        private static void SetEnabledStatus(FrameworkElement expander, bool enabled)
        {
            if (expander.GetChildInTreeOfType<System.Windows.Controls.Primitives.ToggleButton>() != null)
                expander.GetChildInTreeOfType<System.Windows.Controls.Primitives.ToggleButton>().IsEnabled = enabled;
        }
    }
}