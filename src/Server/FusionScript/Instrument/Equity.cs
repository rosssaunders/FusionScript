namespace FusionScript
{
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
}
