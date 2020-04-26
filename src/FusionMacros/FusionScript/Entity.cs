using System;
using sophis.portfolio;
using sophis.misc;
using sophis.tools;
using sophisTools;
using sophis.static_data;
using sophis.utils;
using sophis.backoffice_kernel;

namespace FusionScript
{
    public class ThirdParty
    {
        public ThirdParty(int id)
        {
            this.Id = id;
        }

        public int Id { get; }

        public string Name
        {
            get
            {
                using var p = CSMThirdParty.GetCSRThirdParty(Id);
                using var name = p.GetName();
                return name.StringValue;
            }
        }

        public string Reference
        {
            get
            {
                using var p = CSMThirdParty.GetCSRThirdParty(Id);
                using var reference = new CMString();
                p.GetReference(reference);
                return reference.StringValue;
            }
        }
    }
}
