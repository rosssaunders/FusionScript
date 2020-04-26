using System.Collections.Generic;
using System.ServiceModel;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.Interface
{
    [ServiceContract(CallbackContract = typeof(IDataServiceClient), Namespace = "http://schemas.rxdsolutions.co.uk/fusionscript")]
    public interface IDataService
    {
        [OperationContract]
        void Register();

        [OperationContract]
        void Unregister();

        [OperationContract]
        List<ScriptModel> GetAllScripts();

        [OperationContract]
        ScriptModel GetScript(int id);

        [OperationContract]
        ExecutionResults ExecuteScript(ScriptModel script);

        [OperationContract]
        IEnumerable<string> GetImages();

        [OperationContract]
        bool CanUserExecute();

        [OperationContract]
        bool CanUserManagePrivate();

        [OperationContract]
        bool CanUserManageFirm();

        [OperationContract]
        string GetOwnerName(int ownerId);
        
        [OperationContract]
        void DeleteScript(int id);

        [OperationContract]
        ScriptModel SaveScript(ScriptModel model);
        
        [OperationContract]
        IEnumerable<ScriptExecutionModel> LoadExecutions(int id);

        [OperationContract]
        IEnumerable<ScriptAuditModel> LoadAudit(int id);
        
        [OperationContract]
        IEnumerable<ScriptTriggerAuditModel> LoadTriggerAudit(int id);
        
        [OperationContract]
        IEnumerable<UserModel> LoadUsers();
        
        [OperationContract]
        int GetCurrentUserId();
    }
}
