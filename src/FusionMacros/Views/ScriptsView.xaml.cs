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
            System.Diagnostics.Process.Start(@"https://github.com/rxdsolutions/fusionmacrodocs/wiki");
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.rxdsolutions.co.uk/");
        }
    }
}
