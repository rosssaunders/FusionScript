using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxdSolutions.FusionScript.Runtimes
{
    public interface IExecutionEngine
    {
        string ExecuteScript(string script, bool shutdown = false, IDictionary<string, object> parameters = null);
    }
}
