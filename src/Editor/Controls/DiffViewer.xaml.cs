using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CefSharp;

namespace RxdSolutions.FusionScript.Controls
{
    public partial class DiffViewer : UserControl
    {
        private bool isReady;

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(DiffViewer));

        public static readonly DependencyProperty OriginalTextProperty = DependencyProperty.Register(
                "OriginalText",
                typeof(string),
                typeof(DiffViewer),
                new UIPropertyMetadata("", OnOriginalTextChanged));

        public static readonly DependencyProperty ModifiedTextProperty = DependencyProperty.Register(
                "ModifiedText",
                typeof(string),
                typeof(DiffViewer),
                new UIPropertyMetadata("", OnModifiedTextChanged));

        public string OriginalText
        {
            get
            {
                return (string)GetValue(OriginalTextProperty);
            }
            set
            {
                SetValue(OriginalTextProperty, value);
            }
        }

        public string ModifiedText
        {
            get
            {
                return (string)GetValue(ModifiedTextProperty);
            }
            set
            {
                SetValue(ModifiedTextProperty, value);
            }
        }

        public DiffViewer()
        {
            InitializeComponent();

            this.Browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            this.Browser.LoadingStateChanged += Browser_LoadingStateChanged;

            IsLoading = true;
        }

        private static void OnOriginalTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (DiffViewer)d;
            view.SetComparison();
        }

        private static void OnModifiedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (DiffViewer)d;
            view.SetComparison();
        }


        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Browser.IsBrowserInitialized)
            {
                var editor = System.IO.Path.Combine(Environment.CurrentDirectory, @"monaco\diffviewer.html");
                this.Browser.Load(editor);
            }
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //Wait for the Page to finish loading
                if (e.IsLoading == false)
                {
                    isReady = true;
                    IsLoading = false;

                    if(!string.IsNullOrWhiteSpace(_original) || string.IsNullOrWhiteSpace(_modified))
                    {
                        SetComparison();
                    }    
                }
            });
        }

        private string _original;
        private string _modified;

        /// <summary>
        /// Set's the Text of Monaco to the Parameter text.
        /// </summary>
        /// <param name="text"></param>
        internal void SetComparison()
        {
            _original = OriginalText;
            _modified = ModifiedText;

            if ((this != null) && (isReady))
            {
                Browser.ExecuteScriptAsync("setComparison", new object[] { _original ?? "", _modified ?? "" });
            }
        } 
    }
}
