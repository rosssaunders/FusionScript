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

        private readonly Dictionary<int, ScriptModel> _macros;

        public EventHandler<ScriptUpdateEventArgs> MacroChanged;
        public EventHandler<ScriptUpdateEventArgs> MacroCreated;
        public EventHandler<ScriptUpdateEventArgs> MacroDeleted;

        public DataServiceClient(Uri endPointAddress)
        {
            this.endPointAddress = endPointAddress.ToString();
            _macros = new Dictionary<int, ScriptModel>();
        }

        public ExecutionResults ExecuteScript(ScriptModel model)
        {
            return _server.ExecuteScript(model);
        }

        public Task<ExecutionResults> ExecuteScriptAsync(ScriptModel model)
        {
            return Task.Run(() => _server.ExecuteScript(model));
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

            _callback = new DataServiceClientCallback(MacroCreatedHandler, MacroChangedHandler, MacroDeletedHandler);

            var ep = new EndpointAddress(endPointAddress);

            _df = new DuplexChannelFactory<IDataService>(new InstanceContext(_callback), binding, ep);
            _server = _df.CreateChannel();

            _server.Register();
        }

        private void MacroCreatedHandler(int id, ScriptModel model)
        {
            _macros.Add(id, model);

            MacroCreated?.Invoke(this, new ScriptUpdateEventArgs(id, model));
        }

        private void MacroChangedHandler(int id, ScriptModel model)
        {
            _macros[id] = model;

            MacroChanged?.Invoke(this, new ScriptUpdateEventArgs(id, model));
        }

        private void MacroDeletedHandler(int id, ScriptModel model)
        {
            _macros.Remove(id);

            MacroDeleted?.Invoke(this, new ScriptUpdateEventArgs(id, model));
        }

        public IEnumerable<string> GetImages()
        {
            return _server.GetImages();
        }

        public IEnumerable<ScriptModel> GetAll()
        {
            return _macros.Values;
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

        public ScriptModel GetMacro(int id)
        {
            return _server.GetScript(id);
        }

        public void Load()
        {
            foreach(var model in _server.GetAllScripts())
            {
                _macros.Add(model.Id, model);
            }
        }

        public ScriptModel SaveMacro(ScriptModel model)
        {
            return _server.SaveScript(model);
        }

        public IEnumerable<ScriptExecutionModel> LoadMacroExecutions(int id)
        {
            return _server.LoadExecutions(id);
        }

        public IEnumerable<ScriptAuditModel> LoadMacroAudit(int id)
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
