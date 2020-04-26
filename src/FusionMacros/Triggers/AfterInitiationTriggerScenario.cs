using System.Linq;
using RxdSolutions.FusionScript.Runtimes;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionScript.Triggers
{
    internal class AfterInitiationTriggerScenario : CSMScenario
    {
        public override eMProcessingType GetProcessingType()
        {
            return eMProcessingType.M_pAfterAllInitialisation;
        }

        public override bool AvailableForReport()
        {
            return false;
        }

        public override bool AvailableForScenarioList()
        {
            return false;
        }

        public override bool InTheAnalysisMenu()
        {
            return false;
        }

        public override CMString GetName()
        {
            return "AfterInitiationTriggerScenario";
        }

        public override void Run()
        {
            //Kick off each script
            foreach(var script in Main.ScriptCache.GetAll())
            {
                if (script.Triggers.Any(x => x.Trigger == Model.Trigger.AfterInitialization))
                {
                    var fe = new ExecutionService();
                    fe.ExecuteScript(script, Model.Trigger.AfterInitialization);

                    continue;
                }
            }
        }
    }
}
