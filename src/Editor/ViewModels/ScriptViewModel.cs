using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RxdSolutions.FusionScript.Adapters;
using RxdSolutions.FusionScript.Client;
using RxdSolutions.FusionScript.Controls;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Validation;
using RxdSolutions.FusionScript.Wpf;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class ScriptViewModel : ValidationViewModelBase
    {
        //Model
        private int _id;
        private string _name;
        private string _description;
        private string _script;
        private Language _language;
        private SecurityPermission _securityPermission;
        private int _ownerId;
        private string _icon;

        //Users
        private bool _usersLoaded;

        //UI
        private string _executionResults;
        private string _status;
        private TimeSpan _executionTime;
        private bool _isSelected;
        private bool _isBusy;

        //Runtime
        private readonly DataServiceClient _client;

        //Compare
        private object _selectedAuditForCompare1;
        private object _selectedAuditForCompare2;

        private TriggerViewModel _selectedTrigger;

        public ScriptViewModel(DataServiceClient client)
        {
            SaveCommand = new DelegateCommand(OnSaveCommand, CanSaveCommandExecute);
            ExecuteCommand = new DelegateCommand(OnExecuteCommand);
            AddTriggerCommand = new DelegateCommand(OnAddTriggerCommand);
            RemoveTriggerCommand = new DelegateCommand(OnDeleteTriggerCommand, CanDeleteTriggerCommandExecute);
            RefreshHistoryCommand = new DelegateCommand(OnRefreshHistoryCommand);

            _client = client;

            Audit = new ObservableCollection<AuditViewModel>();
            AuditTrigger = new ObservableCollection<TriggerAuditViewModel>();
            Triggers = new ObservableCollection<TriggerViewModel>();
            TriggerValidationErrors = new ObservableCollection<string>();
            Users = new ObservableCollection<UserModel>();
            History = new ObservableCollection<ScriptExecutionModel>();

            ErrorsChanged += ScriptViewModel_ErrorsChanged;
            Triggers.CollectionChanged += Triggers_CollectionChanged;

            Icons = new ObservableCollection<string>(_client.GetImages().OrderBy(x => x));

            HasUserManageFirmPermissions = _client.CanManageFirm();

            Status = "Ready";
        }

        public bool HasUserManageFirmPermissions { get; }

        public bool IsOwnerEnabled
        {
            get
            {
                return HasUserManageFirmPermissions && SecurityPermission == SecurityPermission.Private;
            }
        }

        public ObservableCollection<string> Icons { get; }

        public ObservableCollection<AuditViewModel> Audit { get; }

        public ObservableCollection<TriggerAuditViewModel> AuditTrigger { get; }

        [UniqueOnLoadTrigger(ErrorMessage = "Only one 'On Load' trigger is allowed to be defined")]
        public ObservableCollection<TriggerViewModel> Triggers { get; }

        public ObservableCollection<string> TriggerValidationErrors { get; }

        public ObservableCollection<UserModel> Users { get; }

        public ObservableCollection<ScriptExecutionModel> History { get; private set; }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand ExecuteCommand { get; }

        public DelegateCommand AddTriggerCommand { get; }

        public DelegateCommand RemoveTriggerCommand { get; }

        public DelegateCommand RefreshHistoryCommand { get; }

        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                SetProperty(ref _status, value);
            }
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

        [Display(Name = "Name")]
        [Required]
        [StringLength(255)]
        [UniqueScriptName(ErrorMessage = "The name must be unique in the system.")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
                ValidateProperty(_name);
            }
        }

        [Display(Name = "Description")]
        [StringLength(2000)]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
                ValidateProperty(_description);

                OnPropertyChanged("DescriptionDisplay");
            }
        }

        [Display(Name = "Description")]
        [StringLength(2000)]
        public string DescriptionDisplay
        {
            get
            {
                if (Description is null)
                    return Description;

                using (var str = new StringReader(Description))
                    return str.ReadLine();
            }
        }

        /// <summary>
        /// Used by the ScriptsViewModel
        /// </summary>
        public string OwnerName
        {
            get
            {
                return _client.GetOwnerName(this.OwnerId);
            }
        }

        [Display(Name = "Script")]
        [Required]
        public string Script
        {
            get
            {
                return _script;
            }
            set
            {
                SetProperty(ref _script, value);
                ValidateProperty(_script);
            }
        }

        [Display(Name = "Language")]
        [Required]
        public Language Language
        {
            get
            {
                return _language;
            }
            set
            {
                SetProperty(ref _language, value);
                ValidateProperty(_language);
            }
        }

        [Display(Name = "Security Permission")]
        [Required]
        public SecurityPermission SecurityPermission
        {
            get
            {
                return _securityPermission;
            }
            set
            {
                SetProperty(ref _securityPermission, value);
                ValidateProperty(_securityPermission);

                OnPropertyChanged("IsOwnerEnabled");
            }
        }

        [Display(Name = "Owner")]
        [Required]
        public int OwnerId
        {
            get
            {
                return _ownerId;
            }
            set
            {
                SetProperty(ref _ownerId, value);
                ValidateProperty(_ownerId);
            }
        }

        [Display(Name = "Icon")]
        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                SetProperty(ref _icon, value);
                ValidateProperty(_icon);
            }
        }

        public object SelectedAudit1
        {
            get
            {
                return _selectedAuditForCompare1;
            }
            set
            {
                SetProperty(ref _selectedAuditForCompare1, value);
                OnPropertyChanged();
                OnPropertyChanged("SelectedAudit1Script");
            }
        }

        public object SelectedAudit1Script
        {
            get
            {
                if (_selectedAuditForCompare1 is AuditViewModel m)
                    return m.Script;

                return Script;
            }
        }

        public object SelectedAudit2
        {
            get
            {
                return _selectedAuditForCompare2;
            }
            set
            {
                SetProperty(ref _selectedAuditForCompare2, value);
                OnPropertyChanged();
                OnPropertyChanged("SelectedAudit2Script");
            }
        }

        public object SelectedAudit2Script
        {
            get
            {
                if (_selectedAuditForCompare2 is AuditViewModel m)
                    return m.Script;

                return Script;
            }
        }

        public TriggerViewModel SelectedTrigger
        {
            get
            {
                return _selectedTrigger;
            }
            set
            {
                SetProperty(ref _selectedTrigger, value);
                RemoveTriggerCommand.RaiseCanExecuteChanged();
            }
        }

        public string ExecutionResults
        {
            get
            {
                return _executionResults + Environment.NewLine;
            }
            set
            {
                SetProperty(ref _executionResults, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }

        public TimeSpan ExecutionTime
        {
            get
            {
                return _executionTime;
            }
            set
            {
                SetProperty(ref _executionTime, value);
            }
        }

        private void Triggers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TriggerViewModel item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= EntityPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TriggerViewModel item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += EntityPropertyChanged;
                }
            }

            Validate();
        }

        private void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Trigger")
                this.ValidateProperty(Triggers, "Triggers");
        }

        private void ScriptViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            TriggerValidationErrors.Clear();
            foreach (string ve in GetErrors("Triggers") ?? Enumerable.Empty<string>())
            {
                TriggerValidationErrors.Add(ve);
            }
            OnPropertyChanged("Triggers");

            SaveCommand.RaiseCanExecuteChanged();
        }

        internal void LoadAudit()
        {
            Audit.Clear();
            AuditTrigger.Clear();

            var records = _client.LoadScriptAudit(this.Id);

            foreach (var r in records.OrderBy(x => x.Version))
            {
                Audit.Add(new AuditViewModel(r));
            }

            if (Audit.Count > 1)
            {
                SelectedAudit1 = Audit.Reverse().Skip(1).Take(1).First();
                SelectedAudit2 = Audit.Last();
            }

            var triggerAudit = _client.LoadTriggerAudit(Id);
            foreach (var r in triggerAudit)
            {
                AuditTrigger.Add(new TriggerAuditViewModel(r));
            }
        }

        internal void LoadHistory()
        {
            History.Clear();

            var records = _client.LoadScriptExecutions(Id);

            foreach (var r in records)
            {
                History.Add(r);
            }
        }

        internal void LoadUsers()
        {
            if (!_usersLoaded)
            {
                var records = _client.LoadUsers();

                foreach (var r in records.OrderBy(x => x.Name))
                {
                    this.Users.Add(r);
                }
            }

            if(_ownerId == 0)
            {
                //Set the current user as the default
                _ownerId = _client.GetCurrentUserId();
            }

            _usersLoaded = true;
        }

        private bool CanSaveCommandExecute(object obj)
        {
            return !this.HasErrors;
        }

        private void OnSaveCommand(object obj)
        {
            if(obj is Editor e)
            {
                this.Script = e.GetText();
            }

            Validate();

            if(this.HasErrors)
            {
                MessageBox.Show("Please fix the validation errors.");
            }
            else
            {
                var adapter = new ViewModelAdapter(_client);
                var model = adapter.Adapt(this);

                try
                {
                    var newModel = _client.SaveScript(model);

                    Id = newModel.Id;
                    adapter.Update(newModel, this);
                    
                    LoadAudit();
                    
                    Status = "Saved";

                    Task.Delay(3000).ContinueWith(t => { if (Status == "Saved") { Status = "Ready"; } }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void OnExecuteCommand(object obj)
        {
            if (obj is Editor e)
            {
                this.Script = e.GetText();
            }

            try
            {
                IsBusy = true;
                Status = "Running script...";

                Mouse.OverrideCursor = Cursors.Wait;

                ExecutionResults = string.Empty;

                var timer = Stopwatch.StartNew();

                var a = new ViewModelAdapter(_client);
                var model = a.Adapt(this);
                model.Script = this.Script;

                var waiter = _client.ExecuteScriptAsync(model);
                waiter.ContinueWith((t) =>
                {
                    if(t.IsCompletedSuccessfully)
                    {
                        timer.Stop();

                        ExecutionResults = t.Result.Results;
                        ExecutionTime = timer.Elapsed;
                    }
                    else
                    {
                        ExecutionResults = t.Exception.ToString();
                    }

                    IsBusy = false;
                    Status = "Ready";

                    Mouse.OverrideCursor = null;

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool CanDeleteTriggerCommandExecute(object obj)
        {
            return SelectedTrigger is object;
        }

        private void OnDeleteTriggerCommand(object obj)
        {
            var index = Triggers.IndexOf(SelectedTrigger);

            var isLastItemInList = Triggers.Count - 1 == index;

            Triggers.Remove(SelectedTrigger);

            if (Triggers.Count == 0)
                return;

            if(isLastItemInList)
                SelectedTrigger = Triggers[index - 1];
            else
                SelectedTrigger = Triggers[index];
        }

        private void OnAddTriggerCommand(object obj)
        {
            var vm = new TriggerViewModel()
            {
                Trigger = Model.Trigger.Schedule
            };

            Triggers.Add(vm);
            this.SelectedTrigger = vm;
        }

        private void OnRefreshHistoryCommand(object obj)
        {
            LoadHistory();
        }
    }
}
