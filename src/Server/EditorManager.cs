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
        private const string _clientExeName = @"FusionScriptEditor.exe";

        private readonly Uri _editorDataUri;
        private readonly int _processId;

        private Process _process;

        public EditorManager(int processId, Uri editorDataUri)
        {
            _processId = processId;
            _editorDataUri = editorDataUri;
        }

        public void OpenView(int? id, bool clone = false)
        {
            var arguments = new List<string>
            {
                $"--server {_editorDataUri}"
            };

            if (id.HasValue)
                arguments.Add($" --id {id.Value}");

            if (clone)
                arguments.Add(" --clone");

            _process = StartProcess(arguments.ToArray());
        }

        public void Exit()
        {
            if(_process is object)
            {
                if(!_process.HasExited)
                {
                    _process = StartProcess($"--shutdown");
                    _process.WaitForExit();
                }
            }
        }

        private Process StartProcess(params string[] arguments)
        {
            var argumentsIncProcessId = new List<string>(arguments)
            {
                $"--processid {_processId}"
            };

            string allArguments = string.Join(" ", argumentsIncProcessId);
            
            //Tell the app to shutdown
            var psi = new ProcessStartInfo
            {
                FileName = _clientExeName,
                Arguments = allArguments,
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, Resources.ToolkitName),
                UseShellExecute = true
            };

            _process = Process.Start(psi);
            return _process;
        }
    }
}
