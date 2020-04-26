using System.Windows.Input;

namespace RxdSolutions.FusionScript.Controls
{
    public class DelegateCommandBinding : CommandBinding
    {
        public ICommand DelegateCommand { get; set; }

        public DelegateCommandBinding()
        {
            this.CanExecute += DelegateCommandBinding_CanExecute;
            this.Executed += DelegateCommandBinding_Executed;
        }

        private void DelegateCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DelegateCommand.Execute(e.Parameter);
        }

        private void DelegateCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DelegateCommand.CanExecute(e.Parameter);
        }
    }
}
