using System;

namespace RxdSolutions.FusionScript.Model
{
    public class ExecutionResults
    {
        public string Results { get; set; }
        
        public ExecutionStatus Status { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
    }
}