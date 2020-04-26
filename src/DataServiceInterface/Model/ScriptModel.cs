using System.Collections.Generic;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptModel
    {
        public ScriptModel()
        {
            Triggers = new List<ScriptTriggerModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Script { get; set; }

        public int OwnerId { get; set; }

        public SecurityPermission SecurityPermission { get; set; }

        public string Icon { get; set; }

        public Language Language { get; set; }

        public List<ScriptTriggerModel> Triggers { get; set; }
    }
}
