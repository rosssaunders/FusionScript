using System;
using sophis.finance;
using sophis.instrument;
using sophis.utils;

namespace FusionMacro
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

        public Bond Bond => new Bond(this);

        public Option Option => new Option(this);

        public ForexNonDeliverableForward ForexNonDeliverableForward => new ForexNonDeliverableForward(this);

        public ForexForward ForexForward => new ForexForward(this);

        public ForexSpot ForexSpot => new ForexSpot(this);

        public Future Future => new Future(this);

        public Equity Equity => new Equity(this);
    }

    public class ForexNonDeliverableForward : Instrument
    {
        public ForexNonDeliverableForward(int id) : base(id)
        {
        }

        public ForexNonDeliverableForward(string reference) : base(reference)
        {
        }

        public ForexNonDeliverableForward(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMNonDeliverableForexForward GetNDFForex()
        {
            CSMNonDeliverableForexForward i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }

    public class ForexForward : Instrument
    {
        public ForexForward(int id) : base(id)
        {
        }

        public ForexForward(string reference) : base(reference)
        {
        }

        public ForexForward(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMForexFuture GetForexForward()
        {
            CSMForexFuture i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }

    public class ForexSpot : Instrument
    {
        public ForexSpot(int id) : base(id)
        {
        }

        public ForexSpot(string reference) : base(reference)
        {
        }

        public ForexSpot(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMForexSpot GetForexSpot()
        {
            CSMForexSpot i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }

    public class Future : Instrument
    {
        public Future(int id) : base(id)
        {
        }

        public Future(string reference) : base(reference)
        {
        }

        public Future(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMFuture GetFuture()
        {
            CSMFuture i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }

    public class Equity : Instrument
    {
        public Equity(int id) : base(id)
        {
        }

        public Equity(string reference) : base(reference)
        {
        }

        public Equity(Instrument instrument) : base(instrument.Id)
        {
        }
    }

    public class Bond : Instrument
    {
        public Bond(int id) : base(id)
        {
        }

        public Bond(string reference) : base(reference)
        {
        }

        public Bond(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMBond GetBond()
        {
            CSMBond i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }

    public class Option : Instrument
    {
        public Option(int id) : base(id)
        {
        }

        public Option(string reference) : base(reference)
        {
        }

        public Option(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMOption GetOption()
        {
            CSMOption i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }
}
