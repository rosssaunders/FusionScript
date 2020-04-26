using System;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptTriggerModel
    {
        public int Id { get; set; }

        public int ScriptId { get; set; }

        public Trigger Trigger { get; set; }

        public DateTime Time { get; set; }
    }
}
