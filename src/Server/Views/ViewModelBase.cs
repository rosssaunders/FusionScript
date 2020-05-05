using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RxdSolutions.FusionScript.Wpf
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string method = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(method));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
