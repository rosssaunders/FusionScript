using System;
using sophis.portfolio;
using sophis.misc;
using sophis.tools;
using sophisTools;

namespace FusionScript
{

    public class Transaction
    {
        public Transaction(long id)
        {
            this.Id = id;
            Version = 0;
        }

        public Transaction(long id, short version)
        {
            this.Id = id;
            this.Version = version;
        }

        public long Id { get; }

        public short Version { get; }

        protected virtual CSMTransaction GetTransaction()
        {
            return new CSMTransaction(Id, Version);
        }

        public DateTime TradeDate
        {
            get
            {
                using var trans = GetTransaction();
                return (DateTime)trans.GetTransactionDate().ToCLRDateTime();
            }
        }

        public DateTime AccountancyDate
        {
            get
            {
                using var trans = GetTransaction();
                return (DateTime)trans.GetAccountancyDate().ToCLRDateTime();
            }
        }

        public Portfolio AccountingBook
        {
            get
            {
                using var trans = GetTransaction();
                return new Portfolio(trans.GetAccountingBook());
            }
        }

        public double AccruedAmount
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetAccruedAmount();
            }
        }

        public double AccruedAmount2
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetAccruedAmount2();
            }
        }

        public double AccruedCoupon
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetAccruedCoupon();
            }
        }

        public object AccruedCouponDate
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetAccruedCouponDate();
            }
        }

        public int Adjustment
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetAdjustment();
            }
        }

        public string AskQuotationType
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetAskQuotationType().ToString();
            }
        }

        public string BackOfficeInfos
        {
            get
            {
                using var trans = GetTransaction();
                using var comment = trans.GetBackOfficeInfos();
                return comment.StringValue;
            }
        }

        public ThirdParty Broker
        {
            get
            {
                using var trans = GetTransaction();
                return new ThirdParty(trans.GetBroker());
            }
        }

        public ThirdParty Entity
        {
            get
            {
                using var trans = GetTransaction();
                return new ThirdParty(trans.GetEntity());
            }
        }

        public ThirdParty Counterparty
        {
            get
            {
                using var trans = GetTransaction();
                return new ThirdParty(trans.GetCounterparty());
            }
        }

        public ThirdParty Counterparty2
        {
            get
            {
                using var trans = GetTransaction();
                return new ThirdParty(trans.GetCounterparty2());
            }
        }

        public ThirdParty Depositary
        {
            get
            {
                using var trans = GetTransaction();
                return new ThirdParty(trans.GetDepositary());
            }
        }

        public double BrokerFees
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetBrokerFees();
            }
        }

        public double CounterpartyFees
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetCounterpartyFees();
            }
        }

        public double MarketFees
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetMarketFees();
            }
        }

        public double Quantity
        {
            get
            {
                using var trans = GetTransaction();
                return trans.GetQuantity();
            }
        }

        public TimeSpan TradeTime
        {
            get
            {
                using var trans = GetTransaction();
                double time = trans.GetTransactionTime();
                var ts = TimeSpan.FromSeconds(time);
                return ts;
            }
        }

        public Instrument Instrument
        {
            get
            {
                using var trans = GetTransaction();
                return new Instrument(trans.GetInstrumentCode());
            }
        }

        public Position Position
        {
            get
            {
                using var trans = GetTransaction();
                return new Position(trans.GetPositionID());
            }
        }
    }
}
