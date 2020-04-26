using System;
using System.Windows;
using System.Windows.Controls;
using CefSharp;

namespace RxdSolutions.FusionScript.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ResultsHtmlView : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ResultsHtmlView), new PropertyMetadata("", OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ResultsHtmlView)d;
            view.LoadResults();
        }

        public ResultsHtmlView()
        {
            InitializeComponent();
        }

        public string Text
        {
            get 
            { 
                return (string)GetValue(TextProperty); 
            }
            set 
            { 
                SetValue(TextProperty, value);
            }
        }

        private void ResultsHtmlView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CefResultsHtml.IsBrowserInitialized)
            {
                CefResultsHtml.Initialized += ResultsHtml_Initialized;
            }
            else
            {
                LoadResults();
            }
        }

        private void ResultsHtml_Initialized(object sender, EventArgs e)
        {
            CefResultsHtml.Initialized -= ResultsHtml_Initialized;

            LoadResults();
        }

        internal void LoadResults()
        {
            if (CefResultsHtml.IsBrowserInitialized)
            {
                var tempOutputFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ExecutionResults.html");
                System.IO.File.WriteAllText(tempOutputFile, "<HTML><BODY>" + Text + "</BODY></HTML>");

                CefResultsHtml.Load(tempOutputFile);
            }
        }
    }
}
