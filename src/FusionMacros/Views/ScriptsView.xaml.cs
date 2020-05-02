using System.Windows.Controls;
using System.Windows.Input;
using RxdSolutions.FusionScript.ViewModels;

namespace RxdSolutions.FusionScript.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ScriptsView : UserControl
    {
        public ScriptsView(ScriptsViewModel scriptsViewModel)
        {
            InitializeComponent();

            this.DataContext = scriptsViewModel;
        }

        private void Help_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/rxdsolutions/fusionscriptdocs/wiki");
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.rxdsolutions.co.uk/");
        }
    }
}
