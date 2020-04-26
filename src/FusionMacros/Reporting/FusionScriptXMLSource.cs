using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionScript.Runtimes;
using sophis.reporting;

namespace RxdSolutions.FusionScript.Reporting
{
    public class FusionScriptXMLSource : CSMXMLSource
    {
        static FusionScriptXMLSource()
        {
        }

        public FusionScriptXMLSource()
        {
            Name = "FusionScriptXMLSource";
            Namespace = "RXD";
        }

        [Persistent]
        public int ScriptId;

        public override void GenerateXMLDescription(DataSet dataSet, bool inPreviewMode, eMGenerationType generationType)
        {
            var factory = new ExecutionEngineFactory();
            var runtime = factory.GetExecutionEngine(Model.Language.Python);

            var parameters = new Dictionary<string, object>();
            parameters.Add("dataSet", dataSet);
            parameters.Add("inPreviewMode", inPreviewMode);
            parameters.Add("generationType", generationType.ToString());

            var script = Main.ScriptCache.GetAll().Single(x => x.Id == 5);

            var results = runtime.ExecuteScript(script.Script, false, parameters);

            //var dt = new DataTable("ScriptResults");
            //dt.Columns.Add("Output");
            //dt.Rows.Add(results);

            //dataSet.Tables.Add(dt);
        }

        public override eMXMLSourceAvailability GetXMLSourceAvailability()
        {
            return eMXMLSourceAvailability.M_ALWAYS;
        }
    }
}
