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

namespace RxdSolutions.FusionScript
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Mutex Mutex;

        private const string applicationName = "FusionScriptEditor";

        private DataServiceClient _client = null;
        private NamedPipeManager _namedPipeManager;

        public App()
        {
            SingleInstanceCheck();
        }

        public void SingleInstanceCheck()
        {
            Mutex = new Mutex(true, applicationName, out bool isOnlyInstance);
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

                var manager = new NamedPipeManager(applicationName);
                manager.Write(filesToOpen);

                Environment.Exit(0);
            }
        }

        public void LoadAppActivator()
        {
            _namedPipeManager = new NamedPipeManager(applicationName);
            _namedPipeManager.StartServer();
            _namedPipeManager.ReceiveString += HandleNamedPipe_OpenRequest;
        }

        public void HandleNamedPipe_OpenRequest(string args)
        {
            Options opt = null;
                Parser.Default.ParseArguments<Options>(args.Split(Environment.NewLine))
                .WithParsed(opts => opt = opts)
                .WithNotParsed((errs) => throw new ApplicationException(string.Join(",", errs)));

            Dispatcher.InvokeAsync(() =>
            {
                if(opt.Shutdown)
                {
                    //Give each window a chance to close gracefully
                    foreach(Window window in this.Windows)
                    {
                        window.Close();
                    }
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

        protected override void OnStartup(StartupEventArgs e)
        {
            Options options = null;

            Parser.Default.ParseArguments<Options>(e.Args)
                .WithParsed(opts => options = opts)
                .WithNotParsed((errs) => throw new ApplicationException(string.Join(",", errs)));

            AppDomain.CurrentDomain.AssemblyResolve += CefSharpInitiailzer.Resolver;

            //Any CefSharp references have to be in another method with NonInlining
            //attribute so the assembly rolver has time to do it's thing.
            CefSharpInitiailzer.Initialize();

            LoadAppActivator();

            InitializeGlobals(options);

            LoadMainWindow(options);
        }

        private void InitializeGlobals(Options options)
        {
            _client = new DataServiceClient(new Uri(options.Server));
            _client.Open();
            _client.Load();
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
                var model = _client.GetMacro(options.ScriptId);
                viewModel = new ViewModelAdapter(_client).Adapt(model);
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
                Shutdown();
            }
        }
    }
}
