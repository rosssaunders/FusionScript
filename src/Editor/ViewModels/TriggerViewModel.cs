using System;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Wpf;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class TriggerViewModel : ValidationViewModelBase
    {
        private Trigger _trigger;
        private DateTime _time;
        private int _id;

        public TriggerViewModel()
        {
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
                ValidateProperty(_id);
            }
        }

        public Trigger Trigger
        {
            get
            {
                return _trigger;
            }
            set
            {
                SetProperty(ref _trigger, value);
                OnPropertyChanged("Description");
                ValidateProperty(_trigger);
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                SetProperty(ref _time, value);
                OnPropertyChanged("Description");
                ValidateProperty(_time);
            }
        }

        public string Description
        {
            get
            {
                if(Trigger == Trigger.Schedule)
                {
                    return $"Executes at {this.Time.ToString("HH:mm:ss")} daily";
                }

                return string.Empty;
            }
        }
    }
}
