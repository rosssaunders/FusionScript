using System.Collections.Generic;
using sophis.portfolio;

namespace FusionScript.Column
{
    /// <summary>
    /// CachedPortfolioColumn
    /// </summary>
    public class CachedPortfolioColumn : PortfolioColumn
    {
        private static Dictionary<string, CachedPortfolioColumnImp> _columnCache = new Dictionary<string, CachedPortfolioColumnImp>();

        public bool RefreshOnPortfolioUpdate { get; set; } = true;

        public bool RefreshOnPositionUpdate { get; set; } = true;

        public bool RefreshOnInstrumentUpdate { get; set; } = true;

        public bool RefreshOnQuotation { get; set; } = true;

        public bool RefreshOnUnknown { get; set; } = true;

        public CachedPortfolioColumn(string name) : base(name)
        {
        }

        public override void Register()
        {
            using var pc = CSMPortfolioColumn.GetCSRPortfolioColumn(Name);
            if (pc is object)
            {
                if (_columnCache.ContainsKey(Name))
                {
                    _columnCache[Name].Settings = this;
                }
            }
            else
            {
                var newPc = new CachedPortfolioColumnImp(this);
                _columnCache.Add(Name, newPc);
                CSMPortfolioColumn.Register(Name, newPc);
            }
        }
    }
}
