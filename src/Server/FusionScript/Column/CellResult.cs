using System;
using sophis.gui;
using sophis.static_data;

namespace FusionScript.Column
{
    public class CellResult
    {
        public static CellResult NullDecimalResult = new CellResult(0d);
        public static CellResult NullStringResult = new CellResult("");
        public static CellResult NullDateTimeResult = new CellResult(default(DateTime));
        public static CellResult NullIntRsult = new CellResult(0);

        private double? _doubleValue;
        private int? _intValue;
        private string _stringValue;
        private DateTime? _dateTimeValue;

        public int? Currency { get; set; }
        
        public CellAlignment? Alignment { get; set; }
        
        public TextStyle? TextStyle { get; set; }
        
        public NullValueType? NullValueType { get; set; }
        
        public int? Decimals { get; set; }

        public CellResult(double value)
        {
            _doubleValue = value;
        }

        public CellResult(double value, int currency) : this(value)
        {
            Currency = currency;
        }

        public CellResult(double value, int currency, CellAlignment alignment) : this(value, currency)
        {
            Alignment = alignment;
        }

        public CellResult(double value, int currency, CellAlignment alignment, int decimals) : this(value, currency, alignment)
        {
            Decimals = decimals;
        }

        public CellResult(double value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle) : this(value, currency, alignment, decimals)
        {
            TextStyle = textStyle;
        }

        public CellResult(double value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle, NullValueType nullValueType) : this(value, currency, alignment, decimals, textStyle)
        {
            NullValueType = nullValueType;
        }

        public CellResult(string value)
        {
            _stringValue = value;
        }

        public CellResult(string value, int currency) : this(value)
        {
            Currency = currency;
        }

        public CellResult(string value, int currency, CellAlignment alignment) : this(value, currency)
        {
            Alignment = alignment;
        }

        public CellResult(string value, int currency, CellAlignment alignment, int decimals) : this(value, currency, alignment)
        {
            Decimals = decimals;
        }

        public CellResult(string value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle) : this(value, currency, alignment, decimals)
        {
            TextStyle = textStyle;
        }

        public CellResult(string value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle, NullValueType nullValueType) : this(value, currency, alignment, decimals, textStyle)
        {
            NullValueType = nullValueType;
        }

        public CellResult(int value)
        {
            _intValue = value;
        }

        public CellResult(int value, int currency) : this(value)
        {
            Currency = currency;
        }

        public CellResult(int value, int currency, CellAlignment alignment) : this(value, currency)
        {
            Alignment = alignment;
        }

        public CellResult(int value, int currency, CellAlignment alignment, int decimals) : this(value, currency, alignment)
        {
            Decimals = decimals;
        }

        public CellResult(int value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle) : this(value, currency, alignment, decimals)
        {
            TextStyle = textStyle;
        }

        public CellResult(int value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle, NullValueType nullValueType) : this(value, currency, alignment, decimals, textStyle)
        {
            NullValueType = nullValueType;
        }

        public CellResult(DateTime value)
        {
            _dateTimeValue = value;
        }

        public CellResult(DateTime value, int currency) : this(value)
        {
            Currency = currency;
        }

        public CellResult(DateTime value, int currency, CellAlignment alignment) : this(value, currency)
        {
            Alignment = alignment;
        }

        public CellResult(DateTime value, int currency, CellAlignment alignment, int decimals) : this(value, currency, alignment)
        {
            Decimals = decimals;
        }

        public CellResult(DateTime value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle) : this(value, currency, alignment, decimals)
        {
            TextStyle = textStyle;
        }

        public CellResult(DateTime value, int currency, CellAlignment alignment, int decimals, TextStyle textStyle, NullValueType nullValueType) : this(value, currency, alignment, decimals, textStyle)
        {
            NullValueType = nullValueType;
        }

        internal void Populate(ref SSMCellValue cellValue, SSMCellStyle cellStyle)
        {
            if(_doubleValue.HasValue)
            {
                cellValue.doubleValue = _doubleValue.Value;
                cellStyle.kind = NSREnums.eMDataType.M_dDouble;

                if (Decimals.HasValue)
                {
                    cellStyle.@decimal = Decimals.Value;
                }
                else
                {
                    cellStyle.@decimal = 4;
                }

                cellStyle.alignment = eMAlignmentType.M_aRight;
            }
            else if(_intValue.HasValue)
            {
                cellValue.integerValue = _intValue.Value;
                cellStyle.kind = NSREnums.eMDataType.M_dInt;
                cellStyle.alignment = eMAlignmentType.M_aRight;
            }
            else if(_stringValue is object)
            {
                cellValue.SetString(_stringValue);
                cellStyle.kind = NSREnums.eMDataType.M_dNullTerminatedString;
                cellStyle.alignment = eMAlignmentType.M_aLeft;
            }
            else if (_dateTimeValue.HasValue)
            {
                cellValue.SetString(_dateTimeValue.Value.ToString("dd-MMM-yyyy"));
                cellStyle.kind = NSREnums.eMDataType.M_dNullTerminatedString;
                cellStyle.alignment = eMAlignmentType.M_aLeft;
            }
            else
            {
                throw new ApplicationException("Unknown data type in CellResult");
            }

            if(Currency.HasValue)
            {
                cellStyle.currency = Currency.Value;

                var color = new SSMRgbColor();
                using var ccyColor = CSMCurrency.GetCSRCurrency(Currency.Value);
                ccyColor.GetRGBColor(color);
                cellStyle.color = color;
            }

            if(Alignment.HasValue)
            {
                cellStyle.alignment = (eMAlignmentType)Alignment.Value;
            }

            if(TextStyle.HasValue)
            {
                cellStyle.style = (eMTextStyleType)TextStyle.Value;
            }

            if(NullValueType.HasValue)
            {
                cellStyle.@null = (eMNullValueType)NullValueType.Value;
            }
        }
    }
}
