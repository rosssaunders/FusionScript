using sophis.instrument;

namespace FusionScript
{
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
    }
}
