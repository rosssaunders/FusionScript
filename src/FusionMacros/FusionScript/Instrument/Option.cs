using sophis.instrument;

namespace FusionScript
{
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
    }
}
