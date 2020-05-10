using System;
using System.Text;
using System.Threading;
using System.Windows;
using CommandLine;
using RxdSolutions.FusionScript.Editors;
using RxdSolutions.FusionScript.Adapters;
using RxdSolutions.FusionScript.Client;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.ViewModels;
using RxdSolutions.FusionScript.Views;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RxdSolutions.FusionScript
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const string applicationName = "FusionScriptEditor";

        private Mutex _singleApplicationMutex;

        private DataServiceClient _client = null;
        private NamedPipeManager _namedPipeManager;

        public void SingleInstanceCheck(int processId)
        {
            var uniqueAppName = $"{applicationName}_{processId}";
            _singleApplicationMutex = new Mutex(true, uniqueAppName, out bool isOnlyInstance);
            if (!isOnlyInstance)
            {
                string filesToOpen = " ";
                var args = Environment.GetCommandLineArgs();
                if (args != null && args.Length > 1)
                {
                    var sb = new StringBuilder();
                    for (int i = 1; i < args.Length; i++)
                    {
                        sb.AppendLine(args[i]);
                    }
                    filesToOpen = sb.ToString();
                }

                var manager = new NamedPipeManager(uniqueAppName);
                manager.Write(filesToOpen);

                Environment.Exit(0);
            }
        }

        public void LoadAppActivator(int processId)
        {
            var uniqueAppName = $"{applicationName}_{processId}";
            _namedPipeManager = new NamedPipeManager(uniqueAppName);
            _namedPipeManager.StartServer();
            _namedPipeManager.ReceiveString += HandleNamedPipe_OpenRequest;
        }

        public void HandleNamedPipe_OpenRequest(object sender, string args)
        {
            Options opt = null;
                Parser.Default.ParseArguments<Options>(args.Split(Environment.NewLine))
                .WithParsed(opts => opt = opts)
                .WithNotParsed((errs) => throw new ApplicationException(string.Join(",", errs)));

            Dispatcher.InvokeAsync(() =>
            {
                if(opt.Shutdown)
                {
                    CloseAllWindowsAndShutdown();
                }
                else
                {
                    if (opt.ScriptId != 0)
                    {
                        foreach (object window in Windows)
                        {
                            if (window is ScriptViewWindow wdw)
                            {
                                if (wdw.CanHandle(opt.ScriptId))
                                {
                                    if (wdw.WindowState == WindowState.Minimized)
                                        wdw.WindowState = WindowState.Normal;

                                    wdw.Topmost = true;
                                    wdw.Activate();

                                    Dispatcher.BeginInvoke(new Action(() => { wdw.Topmost = false; }));

                                    return;
                                }
                            }
                        }
                    }

                    LoadMainWindow(opt);
                }
            });
        }

        private void CloseAllWindowsAndShutdown()
        {
            //Give each window a chance to close gracefully
            foreach (Window window in this.Windows)
            {
                window.Close();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Options options = null;

            Parser.Default.ParseArguments<Options>(e.Args)
                .WithParsed(opts => options = opts)
                .WithNotParsed((errs) => throw new ApplicationException(string.Join(",", errs)));

            SingleInstanceCheck(options.ProcessId);

            LoadAppActivator(options.ProcessId);

            AppDomain.CurrentDomain.AssemblyResolve += CefSharpInitiailzer.Resolver;

            //Any CefSharp references have to be in another method with NonInlining
            //attribute so the assembly rolver has time to do it's thing.
            CefSharpInitiailzer.Initialize();

            InitializeGlobals(options);

            LoadMainWindow(options);
        }

        private void SophisProcess_Exited(object sender, EventArgs e)
        {
            //Sophis has quit or died without informing us. 
            CloseAllWindowsAndShutdown();
        }

        private void InitializeGlobals(Options options)
        {
            _client = new DataServiceClient(new Uri(options.Server));
            _client.Open();
            _client.Load();

            _client.OnScriptExecuting += (s, e) =>
            {
                var sophisProcess = Process.GetProcessById(options.ProcessId);
                sophisProcess.Exited += SophisProcess_Exited;

                SetForegroundWindow(sophisProcess.MainWindowHandle);

            };
        }

        private void LoadMainWindow(Options options)
        {
            ScriptViewModel viewModel;
            if (options.ScriptId == 0)
            {
                viewModel = new ScriptViewModel(_client)
                {
                    Id = 0,
                    Language = Language.Python,
                    SecurityPermission = SecurityPermission.Private
                };
            }
            else
            {
                var model = _client.GetScript(options.ScriptId);
                viewModel = new ViewModelAdapter(_client).Adapt(model);
                
                if(options.Clone)
                {
                    viewModel.Id = 0;
                    viewModel.Name += " (Cloned)";
                }
            }

            viewModel.LoadUsers();

            var window = new ScriptViewWindow(viewModel);
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var keepAlive = false;

            //Is this the last window. Shutdown if so
            foreach(var window in Windows)
            {
                if(window is ScriptViewWindow)
                {
                    keepAlive = true;
                }
            }

            if (!keepAlive)
            {
                _namedPipeManager.StopServer();

                _client.Dispose();

                Shutdown();
            }
        }
    }
}
