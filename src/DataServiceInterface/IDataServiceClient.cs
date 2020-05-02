using System.ServiceModel;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.Interface
{
    [ServiceContract]
    public interface IDataServiceClient
    {
        [OperationContract(IsOneWay = true)]
        void ScriptCreated(int id, ScriptModel model);

        [OperationContract(IsOneWay = true)]
        void ScriptChanged(int id, ScriptModel model);

        [OperationContract(IsOneWay = true)]
        void ScriptDeleted(int id, ScriptModel model);

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Heartbeat();
    }
}
