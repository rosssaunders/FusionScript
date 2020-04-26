using System;
using System.Collections.Generic;
using RxdSolutions.FusionScript.Client;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class ScriptCache
    {
        private readonly DataServiceClient _client;

        private readonly Dictionary<int, ScriptModel> _scriptCache;

        public Action<object, ScriptUpdateEventArgs> ScriptChanged { get; internal set; }
        
        public Action<object, ScriptUpdateEventArgs> ScriptCreated { get; internal set; }
        
        public Action<object, ScriptUpdateEventArgs> ScriptDeleted { get; internal set; }

        public ScriptCache(DataServiceClient client)
        {
            _client = client;
            _scriptCache = new Dictionary<int, ScriptModel>();
        }

        public void Load()
        {
            foreach(var mm in _client.GetAll())
            {
                _scriptCache.Add(mm.Id, mm);
            }
        }

        public IEnumerable<ScriptModel> GetAll()
        {
            return _scriptCache.Values;
        }

        public bool CanManageFirm()
        {
            return _client.CanManageFirm();
        }

        public bool CanManagePrivate()
        {
            return _client.CanManagePrivate();
        }

        public bool CanExecute()
        {
            return _client.CanExecute();
        }
    }
}