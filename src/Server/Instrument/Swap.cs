using sophis.instrument;

namespace FusionScript
{
    public class Swap : Instrument
    {
        public Swap(int id) : base(id)
        {
        }

        public Swap(string reference) : base(reference)
        {
        }

        public Swap(Instrument instrument) : base(instrument.Id)
        {
        }

        public CSMSwap GetSwap()
        {
            CSMSwap i = CSMInstrument.GetInstance(Id);
            return i;
        }
    }
}
