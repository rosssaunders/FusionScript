using System.Collections.Generic;
using sophis.portfolio;

namespace FusionMacro
{
    public class Position
    {
        public Position(int id)
        {
            this.Id = id;
        }

        public int Id { get; }

        public object GetColumnValue(string name)
        {
            var cellValue = new SSMCellValue();
            using var gMain = sophis.globals.CSMExtraction.gMain();
            using var cellStyle = new SSMCellStyle();
            using var position = CSMPosition.GetCSRPosition(Id);
            using var p = CSMPortfolioColumn.GetCSRPortfolioColumn(name);

            if (p is null)
                return $"'{name}' cannot be found.";

            p.GetPositionCell(position, position.GetPortfolioCode(), position.GetPortfolioCode(), gMain, 0, position.GetInstrumentCode(), ref cellValue, cellStyle, false);

            return DataTypeExtensions.ExtractValueFromSophisCell(cellValue, cellStyle);
        }

        public Portfolio Portfolio
        {
            get
            {
                using var position = CSMPosition.GetCSRPosition(Id);
                return new Portfolio(position.GetPortfolioCode());
            }
        }

        public Instrument Instrument {

            get
            {
                using var position = CSMPosition.GetCSRPosition(Id);
                return new Instrument(position.GetInstrumentCode());
            }
        }

        public IEnumerable<Transaction> Transactions
        {
            get
            {
                using var position = CSMPosition.GetCSRPosition(Id);
                var trans = new CSMTransactionVector();
                position.GetTransactions(trans);

                foreach (CSMTransaction transaction in trans)
                {
                    yield return new Transaction(transaction.getInternalCode());
                }
            }
        }
    }
}
