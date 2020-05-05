using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.portfolio;

namespace FusionScript
{
    /// <summary>
    /// Slimed down version of GetPositionCell with just the Position details
    /// </summary>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate void VoteForModificationHandler(Transaction originalTransaction, Transaction modifiedTransaction, int eventId);

    public class TransactionAction
    {
        public TransactionAction(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public VoteForModificationHandler VoteForModification { get; set; }
        
        public void Register()
        {
            var ta = new TransactionActionImp(VoteForModification);
            CSMTransactionAction.Register(Key, CSMTransactionAction.eMOrder.M_oBeforeSophisValidation, ta);
        }
    }

    public class TransactionActionImp : CSMTransactionAction
    {
        private readonly VoteForModificationHandler _modificationHandler;

        public TransactionActionImp(VoteForModificationHandler modificationHandler)
        {
            _modificationHandler = modificationHandler;
        }

        public override void VoteForModification(CSMTransaction original, CSMTransaction transaction, int event_id)
        {
            _modificationHandler?.Invoke(
                    new TransactionWrap(original),
                    new TransactionWrap(transaction),
                    event_id);
        }
    }
}
