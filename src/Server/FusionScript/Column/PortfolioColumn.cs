using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using sophis.instrument;
using sophis.portfolio;

namespace FusionScript.Column
{
    public class PortfolioColumn
    {
        private static Dictionary<string, PortfolioColumnImp> _columnCache = new Dictionary<string, PortfolioColumnImp>();

        public string Name { get; set; }

        public string Group { get; set; }

        public object Portfolio { get; set; } // 

        public object Position { get; set; } //PositionCellValueHandler

        public object Underlying { get; set; } //UnderlyingCellValueHandler

        public PortfolioColumn(string name)
        {
            Name = name;
        }

        public virtual void Register()
        {
            using var pc = CSMPortfolioColumn.GetCSRPortfolioColumn(Name);
            if(pc is object)
            {
                if(_columnCache.ContainsKey(Name))
                {
                    _columnCache[Name].Settings = this;
                }
            }
            else
            {
                var newPc = new PortfolioColumnImp(this);
                _columnCache.Add(Name, newPc);
                CSMPortfolioColumn.Register(Name, newPc);
            }
        }
    }
}
