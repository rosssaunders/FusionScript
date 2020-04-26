using System;
using System.Collections.Generic;
using System.Linq;
using RxdSolutions.FusionScript.Events;
using RxdSolutions.FusionScript.Repository;
using Sophis.Event;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptCache
    {
        private readonly FusionDb db;
        private readonly Dictionary<int, ScriptModel> _cache = new Dictionary<int, ScriptModel>();

        public event EventHandler<ScriptUpdatedEventArgs> ScriptDeleted;
        public event EventHandler<ScriptUpdatedEventArgs> ScriptChanged;
        public event EventHandler<ScriptUpdatedEventArgs> ScriptCreated;

        public ScriptCache(FusionDb db)
        {
            this.db = db;

            var _handler = new SophisEventHandler(ProcessEvent);
            SophisEventManager.Instance.AddHandler(_handler, Thread.MainProcess, Layer.Model);
        }

        public void Load()
        {
            var dbModels = db.LoadAllScripts();

            foreach (var model in dbModels)
            {
                if(!_cache.ContainsKey(model.Id))
                {
                    _cache.Add(model.Id, model);
                }
            }

            foreach(var model in _cache.Keys.ToList())
            {
                var found = false;
                foreach(var m in dbModels)
                {
                    if (model == m.Id)
                        found = true;
                }

                if (!found)
                {
                    _cache.Remove(model);
                    ScriptDeleted?.Invoke(this, new ScriptUpdatedEventArgs(model, null));
                }
            }
        }

        internal IEnumerable<ScriptModel> GetAll()
        {
            return _cache.Values;
        }

        private void ProcessEvent(IEvent evt, ref bool deleteEvent)
        {
            if (evt is UserEvent ue)
            {
                for(int i = 0; i < ue.GetSize(); i++)
                {
                    var tagId = ue.GetTag(i);
                    if(tagId == CoherencyEvents.FusionMacroChanged)
                    {
                        var id = ue.GetLongAt(i);
                        ProcessChangedEvent(id);
                    }
                    else if(tagId == CoherencyEvents.FusionMacroDeleted)
                    {
                        var id = ue.GetLongAt(i);
                        ProcessDeleteEvent(id);
                    }
                }
            }
        }

        private void ProcessChangedEvent(int id)
        {
            if (id != 0)
            {
                var macro = db.LoadScript(id);

                if (macro is null) //We don't have access to this macro
                    return;

                if (_cache.ContainsKey(id))
                {
                    _cache[id] = macro;

                    ScriptChanged?.Invoke(this, new ScriptUpdatedEventArgs(id, macro));
                }
                else
                {
                    _cache.Add(id, macro);

                    ScriptCreated?.Invoke(this, new ScriptUpdatedEventArgs(id, macro));
                }
            }
        }

        private void ProcessDeleteEvent(int id)
        {
            if (_cache.ContainsKey(id))
            {
                _cache.Remove(id);

                ScriptDeleted?.Invoke(this, new ScriptUpdatedEventArgs(id, null));
            }
        }
    }
}
