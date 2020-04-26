using System;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class TriggerAuditViewModel
    {
        private readonly ScriptTriggerAuditModel model;

        public TriggerAuditViewModel(ScriptTriggerAuditModel model)
        {
            this.model = model;
        }

        public DateTime DateModified => model.DateModified;

        public DateTime ? Time
        {
            get
            {
                if (model.Time == DateTime.MinValue)
                    return null;

                return model.Time;
            }
        }
        
        public int Modification => model.Modification;

        public Trigger Trigger => model.Trigger;

        public int Version => model.Version;

        public int UserId => model.UserId;

        public int Id => model.Id;

        public string ModificationName
        {
            get
            {
                return UIHelper.GetModificationName(Modification);
            }
        }

        public string UserName
        {
            get
            {
                return ""; //UIHelper.GetUserName(this.UserId);
            }
        }
    }
}
