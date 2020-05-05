using sophis.instrument;

namespace FusionScript
{
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
    }
}
