using System;
using RxdSolutions.FusionScript.Interface;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.Client
{
    public class DataServiceClientCallback : IDataServiceClient
    {
        private readonly Action<int, ScriptModel> _created;
        private readonly Action<int, ScriptModel> _changed;
        private readonly Action<int, ScriptModel> _deleted;

        public DataServiceClientCallback(Action<int, ScriptModel> created, Action<int, ScriptModel> changed, Action<int, ScriptModel> deleted)
        {
            _created = created;
            _changed = changed;
            _deleted = deleted;
        }

        public void Heartbeat()
        {
            //Do nothing. Just here to keep the Duplex channel alive.
        }

        public void ScriptChanged(int id, ScriptModel model)
        {
            _changed.Invoke(id, model);
        }

        public void ScriptCreated(int id, ScriptModel model)
        {
            _created.Invoke(id, model);
        }

        public void ScriptDeleted(int id, ScriptModel model)
        {
            _deleted.Invoke(id, model);
        }
    }
}
