using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RxdSolutions.FusionScript.Interface;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Properties;
using RxdSolutions.FusionScript.Wpf;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class ScriptsViewModel : ViewModelBase
    {
        private readonly IDataService _service;
        private readonly ScriptCache _scriptCache;
        private readonly EditorManager _editorManager;
        private ScriptModel _scriptViewModel;
        
        public ObservableCollection<ScriptModel> Scripts { get; }

        public ScriptModel SelectedScript
        {
            get
            {
                return _scriptViewModel;
            }
            set
            {
                _scriptViewModel = value;

                OnPropertyChanged();

                NewCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                CloneCommand.RaiseCanExecuteChanged();
                ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand NewCommand { get; }

        public DelegateCommand EditCommand { get; }

        public DelegateCommand DeleteCommand { get; }

        public DelegateCommand CloneCommand { get; }

        public DelegateCommand ExecuteCommand { get; }

        public ScriptsViewModel(IDataService service, ScriptCache scriptCache, EditorManager manager)
        {
            NewCommand = new DelegateCommand(OnNewCommand, OnCanNew);
            EditCommand = new DelegateCommand(OnEditCommand, OnCanTakeActionOnModel);
            DeleteCommand = new DelegateCommand(OnDeleteCommand, OnCanTakeActionOnModel);
            CloneCommand = new DelegateCommand(OnCloneCommand, OnCanTakeActionOnModel);
            ExecuteCommand = new DelegateCommand(OnExecuteCommand, OnCanExecute);
            Scripts = new ObservableCollection<ScriptModel>();

            _service = service;
            _scriptCache = scriptCache;
            _editorManager = manager;

            foreach (var model in _service.GetAllScripts())
            {
                Scripts.Add(model);
            }

            _scriptCache.ScriptChanged += ScriptChanged;
            _scriptCache.ScriptCreated += ScriptChanged;
            _scriptCache.ScriptDeleted += ScriptDeleted;

            HasManageFirmRights = _service.CanUserManageFirm();
            HasManageRights = _service.CanUserManagePrivate() || _service.CanUserManageFirm();
            HasExecuteRights = _service.CanUserExecute();
        }

        public bool HasManageFirmRights { get; } = false;

        public bool HasManageRights { get; } = false;

        public bool HasExecuteRights { get; } = false;

        private void ScriptChanged(object sender, ScriptUpdatedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var existingItem = Scripts.SingleOrDefault(x => x.Id == e.Id);
                if (existingItem is object)
                {
                    var setSelection = false;
                    if(SelectedScript == existingItem)
                    {
                        setSelection = true;
                    }

                    var idx = Scripts.IndexOf(existingItem);
                    Scripts.RemoveAt(idx);
                    Scripts.Insert(idx, e.Model);

                    if (setSelection)
                        SelectedScript = e.Model;
                }
                else
                {
                    Scripts.Add(e.Model);
                }
            });
        }

        private void ScriptDeleted(object sender, ScriptUpdatedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach(var script in Scripts.ToList())
                {
                    if(script.Id == e.Id)
                    {
                        Scripts.Remove(script);
                    }
                }
            });
        }

        private bool OnCanTakeActionOnModel(object obj)
        {
            if (!HasManageRights)
                return false;

            if (SelectedScript is null)
                return false;

            if (SelectedScript.SecurityPermission == SecurityPermission.Firm && !HasManageFirmRights)
            {
                return false;
            }

            return true;
        }

        private bool OnCanNew(object obj)
        {
            return HasManageRights;
        }

        private bool OnCanExecute(object obj)
        {
            if (!HasExecuteRights)
                return false;

            return SelectedScript is object;
        }

        private void OnEditCommand(object obj)
        {
            OpenView(SelectedScript.Id);
        }

        private void OnExecuteCommand(object obj)
        {
            try
            {
                if (!_service.CanUserExecute() && !_service.CanUserManageFirm() && !_service.CanUserManagePrivate())
                    MessageBox.Show(Resources.NoRightsToExecuteScript);

                Mouse.OverrideCursor = Cursors.Wait;

                _service.ExecuteScript(SelectedScript);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void OnDeleteCommand(object obj)
        {
            var result = MessageBox.Show(string.Format(Resources.ConfirmDeleteScript, SelectedScript.Name), Resources.ToolkitName, MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if(result == MessageBoxResult.OK)
            {
                _service.DeleteScript(SelectedScript.Id);
            }
        }

        private void OnNewCommand(object obj)
        {
            OpenView(0);
        }

        private void OnCloneCommand(object obj)
        {
            OpenView(SelectedScript.Id, true);
        }

        private void OpenView(int ? id, bool clone = false)
        {
            _editorManager.OpenView(id, clone);
        }
    }
}
