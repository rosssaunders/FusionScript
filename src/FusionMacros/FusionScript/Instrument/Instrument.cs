using System;
using sophis.finance;
using sophis.instrument;
using sophis.utils;

namespace FusionScript
{
    public class Instrument
    {
        public Instrument(int id)
        {
            Id = id;
        }

        public Instrument(string reference)
        {
            Id = CSMInstrument.GetCodeWithReference(reference);
        }

        public int Id { get; }

        public string Reference 
        {
            get
            {
                using var i = CSMInstrument.GetInstance(Id);
                using var s = i.GetReference();
                return s.StringValue;
            }
        }

        public string Name
        {
            get
            {
                using var i = CSMInstrument.GetInstance(Id);
                using var s = new CMString();
                i.GetName(s);
                return s.StringValue;
            }
        }

        public string Type
        {
            get
            {
                using var i = CSMInstrument.GetInstance(Id);
                return i.GetType_API().ToString();
            }
        }
    }
}
