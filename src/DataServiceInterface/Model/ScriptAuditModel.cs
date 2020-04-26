using System;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptAuditModel : ScriptModel
    {
        public DateTime DateModified { get; set; }

        public int Version { get; set; }

        public int Modification { get; set; }

        public int UserId { get; set; }
    }
}
