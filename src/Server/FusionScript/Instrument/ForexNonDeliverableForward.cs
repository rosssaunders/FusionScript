using sophis.instrument;

namespace FusionScript
{
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
    }
}
