using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using RxdSolutions.FusionScript.Controls;
using RxdSolutions.FusionScript.ViewModels;

namespace RxdSolutions.FusionScript.Views
{
    public partial class ScriptViewWindow : MetroWindow
    {
        public ScriptViewWindow(ScriptViewModel viewModel)
        {
            InitializeComponent();

            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            DataContext = viewModel;

            PreviewKeyDown += ScriptViewWindow_PreviewKeyDown;

            Editor.Text = ViewModel.Script;
        }

        public ScriptViewModel ViewModel => DataContext as ScriptViewModel;

        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            if (window is object)
            {
                window.Close();
            }
        }

        internal bool CanHandle(int macroId)
        {
            if(macroId == (this.DataContext as ScriptViewModel).Id)
            {
                return true;
            }

            return false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private bool _isHistoryLoaded = false;

        private void OnTabSelected(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryLoaded)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var tab = sender as TabItem;
                    if (tab != null)
                    {
                        ViewModel.LoadAudit();
                    }

                    _isHistoryLoaded = true;
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            bool setFocusToEditor = Editor.IsFocused;

            this.ScriptTabControl.SelectedIndex = 0;

            if (setFocusToEditor)
                this.Editor.Focus();
        }


        private void ScriptViewWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.Tab)
                {
                    ResultsTextBox.Focus();
                    return;
                }
            }

            if (e.Key == Key.S && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                this.ViewModel.Script = this.Editor.GetText();
            }
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserShell.OpenBrowser(@"https://github.com/RXDSolutions/FusionScriptDocs/wiki");
        }
    }
}
