using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;

namespace RxdSolutions.FusionScript.Controls
{
    public partial class Editor : UserControl
    {
        private bool isReady;
        private string script;

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(Editor));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                "Text", 
                typeof(string), 
                typeof(Editor), 
                new UIPropertyMetadata("", OnTextChanged));

        public Editor()
        {
            InitializeComponent();

            Browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            Browser.LoadingStateChanged += Browser_LoadingStateChanged;

            LostFocus += Editor_LostFocus;

            IsLoading = true;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s,e) => { return; }));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, (s, e) => { return; }));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (s, e) => { return; }));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, (s, e) => { return; }));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, (s, e) => { return; }));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, (s, e) => { return; }));
        }

        private void Editor_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (Editor)d;
            view.SetText();
        }

        public void UpdateText()
        {
            Text = GetText();
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

        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Browser.IsBrowserInitialized)
            {
                var editor = System.IO.Path.Combine(Environment.CurrentDirectory, @"monaco\editor.html");
                Browser.Load(editor);
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

                    SetText();
                }
            });
        }

        /// <summary>
        /// Set's the Text of Monaco to the Parameter text.
        /// </summary>
        /// <param name="text"></param>
        internal void SetText()
        {
            script = Text;

            if ((this != null) && (isReady))
            {
                if(!string.IsNullOrWhiteSpace(script))
                    Browser.ExecuteScriptAsync("editor.setValue", new object[] { script });
            }
        }

        /// <summary>
        /// Get's the Text of Monaco and returns it.
        /// </summary>
        /// <returns></returns>
        internal string GetText()
        {
            if ((this != null) && (isReady))
            {
                var result = Browser.EvaluateScriptAsync("editor.getValue()").Result;

                script = (string)result.Result;

                return script;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
