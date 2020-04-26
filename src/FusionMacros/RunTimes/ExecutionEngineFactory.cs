using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Runtimes;

namespace RxdSolutions.FusionScript.Runtimes
{
    public class ExecutionEngineFactory
    {
        public IExecutionEngine GetExecutionEngine(Language language)
        {
            switch (language)
            {
                case Language.Python:
                    return new PythonNet3ExecutionEngine();
            }

            throw new ApplicationException($"Unknown language {language}");
        }
    }
}
