using sophis.portfolio;

namespace FusionScript
{
    public class TransactionWrap : Transaction
    {
        private readonly CSMTransaction transaction;

        public TransactionWrap(CSMTransaction transaction) : base(transaction.GetTransactionCode(), (short)transaction.GetAuditVersion())
        {
            this.transaction = transaction;
        }

        protected override CSMTransaction GetTransaction()
        {
            return transaction.Clone();
        }
    }
}
