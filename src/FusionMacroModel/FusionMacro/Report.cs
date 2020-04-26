using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.reporting;

namespace FusionMacro
{
    public class Report
    {
        private Lazy<Dictionary<string, object>> Parameters { get; }


        public Report(string name)
        {
            Name = name;
            Parameters = new Lazy<Dictionary<string, object>>();
        }

        public string Name { get; }

        public string OutputFileName { get; }

        public eMGenerationType GenerationType { get; set; }

        public void AddParameter(string name, object value)
        {
            Parameters.Value.Add(name, value);
        }

        public void GenerateDocument()
        {
            var reportTemplateManager = CSMReportTemplateManager.GetInstance();
            var report = reportTemplateManager.GetReportTemplateWithName(Name);
            
            if(Parameters.IsValueCreated)
            {
                var prm = new ArrayList();
                foreach (var kvp in Parameters.Value)
                {
                    var param = report.GetParameterWithName(kvp.Key);

                    if (param is object)
                    {
                        param.SetValue(Convert.ToString(kvp.Value));
                        prm.Add(param);
                    }
                }

                report.FillParameters(prm);
            }
               
            report.GenerateDocument();
        }
    }
}
