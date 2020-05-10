using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Interface;
using System.Threading.Tasks;

namespace RxdSolutions.FusionScript.Client
{
    public class DataServiceClient : IDisposable
    {
        private readonly string endPointAddress;
        private IDataService _server;
        private DataServiceClientCallback _callback;
        private DuplexChannelFactory<IDataService> _df;

        private readonly Dictionary<int, ScriptModel> _scripts;

        public EventHandler<ScriptUpdateEventArgs> ScriptChanged;
        public EventHandler<ScriptUpdateEventArgs> ScriptCreated;
        public EventHandler<ScriptUpdateEventArgs> ScriptDeleted;

        public EventHandler<ScriptModel> OnScriptExecuting;
        public EventHandler<ExecutionResults> OnScriptExecuted;

        public DataServiceClient(Uri endPointAddress)
        {
            this.endPointAddress = endPointAddress.ToString();
            _scripts = new Dictionary<int, ScriptModel>();
        }

        public ExecutionResults ExecuteScript(ScriptModel model)
        {
            OnScriptExecuting?.Invoke(this, model);

            var results = _server.ExecuteScript(model);

            OnScriptExecuted?.Invoke(this, results);

            return results;
        }

        public Task<ExecutionResults> ExecuteScriptAsync(ScriptModel model)
        {
            return Task.Run(() => ExecuteScript(model));
        }

        public void Open()
        {
            var binding = new NetTcpBinding
            {
                MaxReceivedMessageSize = int.MaxValue
            };
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            _callback = new DataServiceClientCallback(ScriptCreatedHandler, ScriptChangedHandler, ScriptDeletedHandler);

            var ep = new EndpointAddress(endPointAddress);

            _df = new DuplexChannelFactory<IDataService>(new InstanceContext(_callback), binding, ep);
            _server = _df.CreateChannel();

            _server.Register();
        }

        private void ScriptCreatedHandler(int id, ScriptModel model)
        {
            _scripts.Add(id, model);

            ScriptCreated?.Invoke(this, new ScriptUpdateEventArgs(id, model));
        }

        private void ScriptChangedHandler(int id, ScriptModel model)
        {
            _scripts[id] = model;

            ScriptChanged?.Invoke(this, new ScriptUpdateEventArgs(id, model));
        }

        private void ScriptDeletedHandler(int id, ScriptModel model)
        {
            _scripts.Remove(id);

            ScriptDeleted?.Invoke(this, new ScriptUpdateEventArgs(id, model));
        }

        public IEnumerable<string> GetImages()
        {
            return _server.GetImages();
        }

        public IEnumerable<ScriptModel> GetAll()
        {
            return _scripts.Values;
        }

        public bool CanManageFirm()
        {
            return _server.CanUserManageFirm();
        }

        public bool CanManagePrivate()
        {
            return _server.CanUserManagePrivate();
        }

        public bool CanExecute()
        {
            return _server.CanUserExecute();
        }

        public string GetOwnerName(int ownerId)
        {
            return _server.GetOwnerName(ownerId);
        }

        public void DeleteFunction(int id)
        {
            _server.DeleteScript(id);
        }

        public ScriptModel GetScript(int id)
        {
            return _server.GetScript(id);
        }

        public void Load()
        {
            foreach(var model in _server.GetAllScripts())
            {
                _scripts.Add(model.Id, model);
            }
        }

        public ScriptModel SaveScript(ScriptModel model)
        {
            return _server.SaveScript(model);
        }

        public IEnumerable<ScriptExecutionModel> LoadScriptExecutions(int id)
        {
            return _server.LoadExecutions(id);
        }

        public IEnumerable<ScriptAuditModel> LoadScriptAudit(int id)
        {
            return _server.LoadAudit(id);
        }

        public IEnumerable<ScriptTriggerAuditModel> LoadTriggerAudit(int id)
        {
            return _server.LoadTriggerAudit(id);
        }

        public IEnumerable<UserModel> LoadUsers()
        {
            return _server.LoadUsers();
        }

        public int GetCurrentUserId()
        {
            return _server.GetCurrentUserId();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _server.Unregister();
                    }
                    catch
                    { }
                    
                    _df.Close();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
