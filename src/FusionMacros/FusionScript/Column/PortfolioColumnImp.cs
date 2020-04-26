using System;
using sophis.gui;
using sophis.portfolio;
using sophis.utils;

namespace FusionScript.Column
{
    public class PortfolioColumnImp : CSMPortfolioColumn
    {
        public PortfolioColumn Settings { get; set; }

        public PortfolioColumnImp(PortfolioColumn settings)
        {
            Settings = settings;
        }

        public override CMString GetGroup()
        {
            if(!string.IsNullOrWhiteSpace(Settings.Group))
            {
                return Settings.Group;
            }

            return base.GetGroup();
        }

        public override void GetPortfolioCell(int activePortfolioCode, int portfolioCode, CSMExtraction extraction, ref SSMCellValue cellValue, SSMCellStyle cellStyle, bool onlyTheValue)
        {
            try
            {
                if (Settings.Portfolio is null)
                    return;

                CellResult result;
                if (Settings.Position is PortfolioCellValueHandler h)
                {
                    result = h(new Portfolio(portfolioCode));
                }
                else if (Settings.Position is PortfolioCellValueHandlerEx h2)
                {
                    result = h2(new Portfolio(activePortfolioCode), new Portfolio(portfolioCode), new Extraction(extraction.GetInternalID()));
                }
                else
                {
                    throw new ApplicationException("Unknown PortfolioCell function type. Use PortfolioCellValueHandler or PositionCellValueHandlerEx");
                }

                result.Populate(ref cellValue, cellStyle);
            }
            catch (Exception ex)
            {
                PopulateException(ex, ref cellValue, cellStyle);
            }
        }

        public override void GetPositionCell(CSMPosition position, int activePortfolioCode, int portfolioCode, CSMExtraction extraction, int underlyingCode, int instrumentCode, ref SSMCellValue cellValue, SSMCellStyle cellStyle, bool onlyTheValue)
        {
            try
            {
                if (Settings.Position is null)
                    return;

                CellResult result;
                if (Settings.Position is PositionCellValueHandler h)
                {
                    result = h(new Position(position.GetIdentifier()));
                }
                else if (Settings.Position is PositionCellValueHandlerEx h2)
                {
                    result = h2(new Position(position.GetIdentifier()), new Portfolio(activePortfolioCode), new Portfolio(portfolioCode), new Extraction(extraction.GetInternalID()), new Instrument(underlyingCode), new Instrument(instrumentCode));
                }
                else
                {
                    throw new ApplicationException("Unknown PositionCell function type. Use PositionCellValueHandler or PositionCellValueHandlerEx");
                }

                result.Populate(ref cellValue, cellStyle);
            }
            catch (Exception ex)
            {
                PopulateException(ex, ref cellValue, cellStyle);
            }
        }

        public override void GetUnderlyingCell(int activePortfolioCode, int portfolioCode, CSMExtraction extraction, int underlyingCode, ref SSMCellValue cellValue, SSMCellStyle cellStyle, bool onlyTheValue)
        {
            try
            {
                if (Settings.Underlying is null)
                    return;

                CellResult result;
                if (Settings.Position is UnderlyingCellValueHandler h)
                {
                    result = h(new Instrument(underlyingCode));
                }
                else if (Settings.Position is UnderlyingCellValueHandlerEx h2)
                {
                    result = h2(new Portfolio(activePortfolioCode), new Portfolio(portfolioCode), new Extraction(extraction.GetInternalID()), new Instrument(underlyingCode));
                }
                else
                {
                    throw new ApplicationException("Unknown UnderlyingCell function type. Use UnderlyingCellValueHandler or UnderlyingCellValueHandlerEx");
                }

                result.Populate(ref cellValue, cellStyle);
            }
            catch (Exception ex)
            {
                PopulateException(ex, ref cellValue, cellStyle);
            }
        }

        private void PopulateException(Exception ex, ref SSMCellValue cellValue, SSMCellStyle cellStyle)
        {
            cellValue.SetString(ex.Message);
            cellStyle.kind = NSREnums.eMDataType.M_dNullTerminatedString;
            cellStyle.alignment = eMAlignmentType.M_aLeft;
        }
    }
}
