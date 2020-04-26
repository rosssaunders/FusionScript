using System.ServiceModel;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.Interface
{
    [ServiceContract]
    public interface IDataServiceClient
    {
        [OperationContract(IsOneWay = true)]
        void MacroCreated(int macroId, ScriptModel model);

        [OperationContract(IsOneWay = true)]
        void MacroChanged(int macroId, ScriptModel model);

        [OperationContract(IsOneWay = true)]
        void MacroDeleted(int macroId, ScriptModel model);

        [OperationContract]
        [FaultContract(typeof(ErrorFaultContract))]
        void Heartbeat();
    }
}
