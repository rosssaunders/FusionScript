using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Wpf;

namespace RxdSolutions.FusionScript.Reporting
{
    public class FusionScriptReportingSourceGUIViewModel : ViewModelBase
    {
        private readonly ScriptCache _cache;
        private int _scriptId;

        public FusionScriptReportingSourceGUIViewModel(ScriptCache cache)
        {
            _cache = cache;

            foreach(var script in _cache.GetAll())
            {
                Scripts.Add(script);
            }
        }

        public int ScriptId
        {
            get
            {
                return _scriptId;
            }
            set
            {
                SetProperty(ref _scriptId, value);
            }
        }

        public ObservableCollection<ScriptModel> Scripts { get; } = new ObservableCollection<ScriptModel>();
    }
}
