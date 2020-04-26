using System;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Wpf;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class ExecutionViewModel : ViewModelBase
    {
        private Trigger _trigger;
        private int _id;
        private int _statusId;
        private string _results;
        private DateTime _startedAt;
        private DateTime _endedAt;

        public ExecutionViewModel()
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
            }
        }

        public DateTime StartedAt
        {
            get
            {
                return _startedAt;
            }
            set
            {
                SetProperty(ref _startedAt, value);
            }
        }

        public DateTime EndedAt
        {
            get
            {
                return _endedAt;
            }
            set
            {
                SetProperty(ref _endedAt, value);
            }
        }

        public string Status
        {
            get
            {
                return _statusId.ToString();
            }
            set
            {
                SetProperty(ref _statusId, Convert.ToInt32(value));
            }
        }

        public string Results
        {
            get
            {
                return _results;
            }
            set
            {
                SetProperty(ref _results, value);
            }
        }
    }
}
