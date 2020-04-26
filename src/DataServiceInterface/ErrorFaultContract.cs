using System.Runtime.Serialization;

namespace RxdSolutions.FusionScript.Interface
{
    [DataContract]
    public class ErrorFaultContract
    {
        [DataMember]
        public string Message { get; set; }
    }
}
