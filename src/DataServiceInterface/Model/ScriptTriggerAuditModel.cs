using System;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptTriggerAuditModel : ScriptTriggerModel
    {
        public DateTime DateModified { get; set; }

        public int Version { get; set; }

        public int Modification { get; set; }

        public int UserId { get; set; }
    }
}
