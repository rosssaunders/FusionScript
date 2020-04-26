using System;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptExecutionModel
    {
        public ScriptExecutionModel()
        {
        }

        public int Id { get; set; }

        public int ScriptId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime EndedAt { get; set; }

        public Trigger Trigger { get; set; }

        public string Results { get; set; }

        public ExecutionStatus Status { get; set; }

        public TimeSpan ExecutionTime
        {
            get
            {
                return EndedAt - StartedAt;
            }
        }
    }
}