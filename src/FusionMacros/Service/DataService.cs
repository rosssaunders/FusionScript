using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Threading;
using RxdSolutions.FusionScript.Events;
using RxdSolutions.FusionScript.Interface;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Repository;
using RxdSolutions.FusionScript.Runtimes;
using RxdSolutions.FusionScript.Security;
using sophis.misc;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionScript.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DataService : IDataService
    {
        private readonly Dictionary<string, IDataServiceClient> _clients;

        private readonly ScriptCache _cache;
        private readonly ExecutionService _engine;

        private readonly Dispatcher _context;

        private readonly AutoResetEvent _clientMonitorResetEvent;
        private readonly int _clientCheckInterval;
        private Thread _clientMonitorThread;
        private bool _isClosed = false;

        public DataService(Dispatcher context, ScriptCache cache, ExecutionService engine)
        {
            _context = context;

            _clients = new Dictionary<string, IDataServiceClient>();

            _cache = cache;
            _engine = engine;

            _cache.ScriptChanged += CacheScriptChanged;
            _cache.ScriptCreated += CacheScriptCreated;
            _cache.ScriptDeleted += CacheScriptDeleted;

            _clientMonitorResetEvent = new AutoResetEvent(false);
            _clientCheckInterval = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;
            _clientMonitorThread = new Thread(new ThreadStart(CheckClientsAlive));
            _clientMonitorThread.Name = "FusionScriptClientConnectionMonitor";
            _clientMonitorThread.Start();
        }


        internal void Stop()
        {
            _isClosed = true;
            _clientMonitorResetEvent.Set();
            _clientMonitorThread.Join();
        }

        public void Register()
        {
            try
            {
                var c = OperationContext.Current.GetCallbackChannel<IDataServiceClient>();

                lock (_clients)
                {
                    if (!_clients.ContainsKey(OperationContext.Current.SessionId))
                        _clients.Add(OperationContext.Current.SessionId, c);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void Unregister()
        {
            try
            {
                lock (_clients)
                {
                    if (_clients.ContainsKey(OperationContext.Current.SessionId))
                        _clients.Remove(OperationContext.Current.SessionId);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public List<ScriptModel> GetAllScripts()
        {
            try
            {
                return _context.Invoke(() =>
                {
                    return _cache.GetAll().ToList();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public ScriptModel GetScript(int id)
        {
            try
            {
                return _context.Invoke(() =>
                {
                    return _cache.GetAll().SingleOrDefault(x => x.Id == id);
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public ExecutionResults ExecuteScript(ScriptModel model)
        {
            try
            {
                ExecutionResults ExecuteScriptInternal()
                {
                    return _engine.ExecuteScript(model);
                }

                return _context.Invoke(() =>
                {
                    return ExecuteScriptInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public string GetOwnerName(int ownerId)
        {
            try
            {
                string GetOwnerNameInternal()
                {
                    using var userRight = new CSMUserRights((uint)ownerId);
                    using var str = userRight.GetName();
                    return str.StringValue;
                }

                return _context.Invoke(() =>
                {
                    return GetOwnerNameInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public IEnumerable<string> GetImages()
        {
            try
            { 
                IEnumerable<string> GetImagesInternal()
                {
                    var results = new List<string>();
                    foreach (var ri in RibbonImages.GetLargeImages())
                    {
                        results.Add(ri.Name);
                    }

                    return results;
                }

                return _context.Invoke(() =>
                {
                    return GetImagesInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public bool CanUserExecute()
        {
            try
            {
                bool CanExecuteInternal()
                {
                    return UserRights.CanExecute();
                }

                return _context.Invoke(() =>
                {
                    return CanExecuteInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public bool CanUserManagePrivate()
        {
            try
            { 
                bool CanManagePrivateInternal()
                {
                    return UserRights.CanManagePrivate();
                }

                return _context.Invoke(() =>
                {
                    return CanManagePrivateInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public bool CanUserManageFirm()
        {
            try
            {
                bool CanManageFirmInternal()
                {
                    return UserRights.CanManageFirm();
                }

                return _context.Invoke(() =>
                {
                    return CanManageFirmInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public void DeleteScript(int id)
        {
            try
            {
                void DeleteInternal()
                {
                    var fusionDb = new FusionDb();
                    fusionDb.DeleteScript(id);

                    var x = new CSMUserEvent();
                    x.Add(CoherencyEvents.FusionScriptDeleted, id);
                    CSMGlobalFunctions.GetCurrentGlobalFunctions().SendEvent(x);
                }

                _context.Invoke(() =>
                {
                    DeleteInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public ScriptModel SaveScript(ScriptModel model)
        {
            try
            {
                ScriptModel SaveInternal()
                {
                    var fusionDb = new FusionDb();
                    fusionDb.SaveScript(model);

                    var x = new CSMUserEvent();
                    x.Add(CoherencyEvents.FusionScriptChanged, model.Id);
                    CSMGlobalFunctions.GetCurrentGlobalFunctions().SendEvent(x);

                    return model;
                }

                return _context.Invoke(() =>
                {
                    return SaveInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public IEnumerable<ScriptExecutionModel> LoadExecutions(int id)
        {
            try
            {

                IEnumerable<ScriptExecutionModel> LoadExecutions()
                {
                    var fusionDb = new FusionDb();
                    return fusionDb.LoadScriptExecutions(id);
                }

                return _context.Invoke(() =>
                {
                    return LoadExecutions();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public IEnumerable<ScriptAuditModel> LoadAudit(int id)
        {
            try
            {
                IEnumerable<ScriptAuditModel> LoadAuditInternal()
                {
                    var fusionDb = new FusionDb();
                    return fusionDb.LoadScriptAudit(id);
                }

                return _context.Invoke(() =>
                {
                    return LoadAuditInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public IEnumerable<ScriptTriggerAuditModel> LoadTriggerAudit(int id)
        {
            try
            {
                IEnumerable<ScriptTriggerAuditModel> LoadTriggerAuditInternal()
                {
                    var fusionDb = new FusionDb();
                    return fusionDb.LoadTriggerAudit(id);
                }

                return _context.Invoke(() =>
                {
                    return LoadTriggerAuditInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public IEnumerable<UserModel> LoadUsers()
        {
            try
            {
                IEnumerable<UserModel> LoadUsersInternal()
                {
                    var fusionDb = new FusionDb();
                    return fusionDb.LoadUsers();
                }

                return _context.Invoke(() =>
                {
                    return LoadUsersInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        public int GetCurrentUserId()
        {
            try
            {
                int GetCurrentUserIdInternal()
                {
                    return UserRightHelper.GetCurrentUserId();
                }

                return _context.Invoke(() =>
                {
                    return GetCurrentUserIdInternal();
                });
            }
            catch (Exception ex)
            {
                throw new FaultException<ErrorFaultContract>(new ErrorFaultContract() { Message = ex.Message });
            }
        }

        private void CheckClientsAlive()
        {
            while (!_isClosed)
            {
                SendMessageToAllClients((sessionId, client) => client.Heartbeat());

                _clientMonitorResetEvent.WaitOne(_clientCheckInterval);
            }
        }

        private void CacheScriptDeleted(object sender, ScriptUpdatedEventArgs e)
        {
            SendMessageToAllClients((sessionId, client) => client.ScriptDeleted(e.Id, e.Model));
        }

        private void CacheScriptCreated(object sender, ScriptUpdatedEventArgs e)
        {
            SendMessageToAllClients((sessionId, client) => client.ScriptCreated(e.Id, e.Model));
        }

        private void CacheScriptChanged(object sender, ScriptUpdatedEventArgs e)
        {
            SendMessageToAllClients((sessionId, client) => client.ScriptChanged(e.Id, e.Model));
        }

        private void SendMessageToAllClients(Action<string, IDataServiceClient> send)
        {
            lock (_clients)
            {
                if (_clients.Count == 0)
                    return;

                // send number to clients
                foreach (var key in _clients.Keys.ToList())
                {
                    var c = _clients[key];

                    try
                    {
                        send.Invoke(key, c);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.ToString());

                        lock (_clients)
                        {
                            if (_clients.ContainsKey(key))
                                _clients.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
