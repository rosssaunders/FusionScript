using System;
using sophis.gui;
using sophis.portfolio;
using sophis.utils;

namespace FusionScript.Column
{
    public class CachedPortfolioColumnImp : CSMCachedPortfolioColumn
    {
        public CachedPortfolioColumn Settings { get; set; }

        public CachedPortfolioColumnImp(CachedPortfolioColumn settings)
        {
            Settings = settings;
        }

        public override bool InvalidationNeeded(ref MEvent.MType @event)
        {
            switch (@event)
            {
                case MEvent.MType.M_PortfolioUpdate:
                    return Settings.RefreshOnPortfolioUpdate;
                case MEvent.MType.M_PositionUpdate:
                    return Settings.RefreshOnPositionUpdate;
                case MEvent.MType.M_InstrumentUpdate:
                    return Settings.RefreshOnInstrumentUpdate;
                case MEvent.MType.M_Quotation:
                    return Settings.RefreshOnQuotation;
                case MEvent.MType.M_Unknown:
                    return Settings.RefreshOnUnknown;
            }

            return false;
        }

        public override CMString GetGroup()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Group))
                return Settings.Group;

            return base.GetGroup();
        }

        private void PopulateException(Exception ex, ref SSMCellValue cellValue, SSMCellStyle cellStyle)
        {
            cellValue.SetString(ex.Message);
            cellStyle.kind = NSREnums.eMDataType.M_dNullTerminatedString;
            cellStyle.alignment = eMAlignmentType.M_aLeft;
        }

        public override void ComputePortfolioCell(SSMCellKey key, ref SSMCellValue value, SSMCellStyle style)
        {
            try
            {
                if (Settings.Portfolio is null)
                    return;

                CellResult result;
                if (Settings.Position is PortfolioCellValueHandler h)
                {
                    result = h(new Portfolio(key.PortfolioCode()));
                }
                else if (Settings.Position is PortfolioCellValueHandlerEx h2)
                {
                    result = h2(new Portfolio(key.ActivePortfolioCode()), new Portfolio(key.PortfolioCode()), new Extraction(key.ExtractionId()));
                }
                else
                {
                    throw new ApplicationException("Unknown PortfolioCell function type. Use PortfolioCellValueHandler or PositionCellValueHandlerEx");
                }

                result.Populate(ref value, style);
            }
            catch (Exception ex)
            {
                PopulateException(ex, ref value, style);
            }

            base.ComputePortfolioCell(key, ref value, style);
        }

        public override void ComputePositionCell(SSMCellKey key, ref SSMCellValue value, SSMCellStyle style)
        {
            try
            {
                if (Settings.Position is null)
                    return;

                CellResult result;
                if(Settings.Position is PositionCellValueHandler h)
                {
                    result = h(new Position(key.PositionId()));
                }
                else if(Settings.Position is PositionCellValueHandlerEx h2)
                {
                    result = h2(new Position(key.PositionId()), new Portfolio(key.ActivePortfolioCode()), new Portfolio(key.PortfolioCode()), new Extraction(key.ExtractionId()), new Instrument(key.UnderlyingCode()), new Instrument(key.InstrumentCode()) );
                }
                else
                {
                    throw new ApplicationException("Unknown PositionCell function type. Use PositionCellValueHandler or PositionCellValueHandlerEx");
                }

                result.Populate(ref value, style);
            }
            catch (Exception ex)
            {
                PopulateException(ex, ref value, style);
            }
        }

        public override void ComputeUnderlyingCell(SSMCellKey key, ref SSMCellValue value, SSMCellStyle style)
        {
            try
            {
                if (Settings.Underlying is null)
                    return;

                CellResult result;
                if (Settings.Position is UnderlyingCellValueHandler h)
                {
                    result = h(new Instrument(key.UnderlyingCode()));
                }
                else if (Settings.Position is UnderlyingCellValueHandlerEx h2)
                {
                    result = h2(new Portfolio(key.ActivePortfolioCode()), new Portfolio(key.PortfolioCode()), new Extraction(key.ExtractionId()), new Instrument(key.UnderlyingCode()));
                }
                else
                {
                    throw new ApplicationException("Unknown UnderlyingCell function type. Use UnderlyingCellValueHandler or UnderlyingCellValueHandlerEx");
                }

                result.Populate(ref value, style);
            }
            catch (Exception ex)
            {
                PopulateException(ex, ref value, style);
            }
        }
    }
}
