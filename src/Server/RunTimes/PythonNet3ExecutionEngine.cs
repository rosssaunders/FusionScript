using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Python.Runtime;
using RxdSolutions.FusionScript.Security;

namespace RxdSolutions.FusionScript.Runtimes
{
    public class PythonNet3ExecutionEngine : IExecutionEngine
    {
        static bool _hasFullToolkitAccess = false;

        static PythonNet3ExecutionEngine()
        {
            _hasFullToolkitAccess = UserRights.CanAccessFullToolkit();
        }
        
        public string ExecuteScript(string script, bool shutdown = false, IDictionary<string, object> parameters = null)
        {
            if (script is null)
                throw new ArgumentNullException("script");

            Initialize();

            try
            {
                using (Py.GIL())
                {
                    dynamic sys = PythonEngine.ImportModule("sys");
                    string codeToRedirectOutput =
                        "import sys\n" +
                        "from io import StringIO\n" +
                        "sys.stdout = mystdout = StringIO()\n" +
                        "sys.stdout.flush()\n" +
                        "sys.stderr = mystderr = StringIO()\n" +
                        "sys.stderr.flush()\n";

                    if(parameters is object)
                    {
                        using (PyScope scope = Py.CreateScope())
                        {
                            if (parameters is object)
                                foreach (var p in parameters)
                                {
                                    PyObject ds = p.Value.ToPython();
                                    scope.Set(p.Key, ds);
                                }

                            scope.Exec(codeToRedirectOutput);

                            scope.Exec(script);

                            string pyStdout = sys.stdout.getvalue(); // Get stdout
                            pyStdout = pyStdout.Replace("\n", "\r\n");

                            return pyStdout;
                        }
                    }
                    else
                    {
                        PythonEngine.Exec(codeToRedirectOutput);

                        PythonEngine.Exec(script);

                        string pyStdout = sys.stdout.getvalue(); // Get stdout
                        pyStdout = pyStdout.Replace("\n", "\r\n");

                        return pyStdout;
                    }
                }

                throw new ApplicationException("PythonEngine not initialized");
            }
            catch (PythonException ex)
            {
                throw new ExecutionException(ex.Message);
            }
            finally
            {
                if (shutdown)
                {
                    if (PythonEngine.IsInitialized)
                    {
                        PythonEngine.OnModuleImporting -= PythonEngine_OnModuleImporting;
                        PythonEngine.OnModuleImported -= PythonEngine_OnModuleImported;
                        PythonEngine.Shutdown();
                    }
                }
            }
        }

        private static void PythonEngine_OnModuleImporting(object sender, ImportEventArgs e)
        {
            if (e.ModuleName.StartsWith("sophis."))
            {
                if(!_hasFullToolkitAccess)
                    throw new ApplicationException("You do not have the rights to access sophis modules");
            }
        }

        private static void PythonEngine_OnModuleImported(object sender, ImportEventArgs e)
        {
            Debug.Print(e.ModuleName);
        }

        public static void Initialize()
        {
            if (!PythonEngine.IsInitialized)
            {
                PythonEngine.Initialize();

                PythonEngine.OnModuleImporting += PythonEngine_OnModuleImporting;
                PythonEngine.OnModuleImported += PythonEngine_OnModuleImported;
            }
        }
    }
}
