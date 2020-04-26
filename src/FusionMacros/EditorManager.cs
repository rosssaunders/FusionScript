using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RxdSolutions.FusionScript.Properties;

namespace RxdSolutions.FusionScript
{
    public class EditorManager
    {
        string _editorDataUri;
        Process _process;

        public EditorManager(string editorDataUri)
        {
            _editorDataUri = editorDataUri;
        }

        public void OpenView(int? id, bool clone = false)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"FusionScriptEditor.exe";
            psi.Arguments = $"--server {_editorDataUri}";
            psi.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, Resources.ToolkitName);

            if (id.HasValue)
                psi.Arguments += $" --id {id.Value}";

            if (clone)
                psi.Arguments += " --clone";

            _process = Process.Start(psi);
        }

        public void Exit()
        {
            if(_process is object)
            {
                if(!_process.HasExited)
                {
                    //Tell the app to shutdown
                    var psi = new ProcessStartInfo();
                    psi.FileName = @"FusionScriptEditor.exe";
                    psi.Arguments = $"--shutdown";
                    psi.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, Resources.ToolkitName);
                    _process = Process.Start(psi);
                    _process.WaitForExit();
                }
            }
        }
    }
}
