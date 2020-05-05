using sophis.instrument;

namespace FusionScript
{
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
    }
}
