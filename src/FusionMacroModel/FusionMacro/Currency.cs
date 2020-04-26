using sophis.misc;
using sophis.tools;
using sophisTools;
using sophis.static_data;
using sophis.utils;
using sophis.scenario;
using sophis.market_data;
using sophis.instrument;

namespace FusionMacro
{
    public class Currency
    {
        public Currency(int id)
        {
            this.Id = id;
        }

        public Currency(string isoCode)
        {
            this.Id = CSMCurrency.StringToCurrency(isoCode);
        }

        public int Id { get; }
        
        public string Name
        {
            get
            {
                using var p = CSMCurrency.GetCSRCurrency(Id);
                using var name = new CMString();
                p.GetName(name);
                return name.StringValue;
            }
        }

        public string IsoCode
        {
            get
            {
                using var name = new CMString();
                CSMCurrency.CurrencyToString(Id, name);
                return name.StringValue;
            }
        }
    }
}
