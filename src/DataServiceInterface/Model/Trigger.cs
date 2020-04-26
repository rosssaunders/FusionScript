using System.ComponentModel;

namespace RxdSolutions.FusionScript.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Trigger
    {
        [Description("On system load")]
        Load = 1,

        [Description("On defined schedule")]
        Schedule = 2,

        [Description("After initialization")]
        AfterInitialization = 3,
    }
}
