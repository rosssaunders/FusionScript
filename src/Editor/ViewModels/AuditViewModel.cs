using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class AuditViewModel
    {
        private readonly ScriptAuditModel model;

        public AuditViewModel(ScriptAuditModel model)
        {
            this.model = model;
        }

        public DateTime DateModified => model.DateModified;

        public string Description => model.Description;

        public Language Language => model.Language;

        public int Modification => model.Modification;

        public string Name => model.Name;

        public int OwnerId => model.OwnerId;

        public string Script => model.Script;

        public SecurityPermission SecurityPermission => model.SecurityPermission;

        public int Version => model.Version;

        public int UserId => model.UserId;

        public string ModificationName
        {
            get
            {
                return UIHelper.GetModificationName(Modification);
            }
        }

        public string OwnerName
        {
            get
            {
                return ""; //UIHelper.GetUserName(this.OwnerId);
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
