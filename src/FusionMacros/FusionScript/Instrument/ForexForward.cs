using sophis.instrument;

namespace FusionScript
{
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
    }
}
