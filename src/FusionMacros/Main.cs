using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using RxdSolutions.FusionScript.Client;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Properties;
using RxdSolutions.FusionScript.Reporting;
using RxdSolutions.FusionScript.Repository;
using RxdSolutions.FusionScript.Runtimes;
using RxdSolutions.FusionScript.Security;
using RxdSolutions.FusionScript.Service;
using RxdSolutions.FusionScript.Triggers;
using sophis;
using sophis.misc;
using sophis.reporting;
using sophis.reporting.gui;
using sophis.scenario;
using sophis.utils;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionScript
{
    public class Main : IMain
    {
        public static ScriptCache ScriptCache;
        public static ExecutionEngineFactory ExecutionEngineFactory;
        public static ExecutionService ExecutionEngine;

        private static Scheduler _scheduler;
        private static FusionScriptCommandManager _commandManager;

        private static ServiceHost _host;
        private static DataServiceClient _client;
        private static EditorManager _editorManager;

        public void EntryPoint()
        {
            try
            {
                if(!UserRights.CanExecute() && !UserRights.CanManagePrivate() && !UserRights.CanManageFirm())
                {
                    CSMLog.Write(nameof(Main), nameof(EntryPoint), CSMLog.eMVerbosity.M_info, Resources.NoPermissionToLoadFusionScript);

                    return;
                }

                _editorManager = new EditorManager(DataServerHostFactory.GetListeningAddress().ToString());

                InitialiseCache();

                RegisterScenarios();

                RegisterReportingModuleExtensions();

                InitialiseMenu();

                RegisterExtensionLocations();

                ExecutionEngineFactory = new ExecutionEngineFactory();
                ExecutionEngine = new ExecutionService();

                _scheduler = new Scheduler(ScriptCache, Dispatcher.CurrentDispatcher, ExecutionEngine);
                _scheduler.Start();

                RegisterServerAndClient();

                PythonNet3ExecutionEngine.Initialize();
            }
            catch (Exception ex)
            {
                CSMLog.Write(nameof(Main), nameof(EntryPoint), CSMLog.eMVerbosity.M_error, ex.ToString());
            }

            try
            {
                ExecuteScriptOnloadTriggers();
            }
            catch (Exception ex)
            {
                CSMLog.Write(nameof(Main), nameof(EntryPoint), CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        private void RegisterReportingModuleExtensions()
        {
            CSMXMLSource.Register("FusionScript", new FusionScriptXMLSource());
            CSMXMLSourceGUI.Register("FusionScript", new FusionScriptXMLSourceGUI());
        }

        public void Close()
        {
            try
            {
                _editorManager?.Exit();

                _scheduler.Stop();

                if(_host is object)
                {
                    var ds = _host.SingletonInstance as DataService;
                    ds.Stop();
                    _host.Close();
                }
            }
            catch (Exception ex)
            {
                CSMLog.Write("Main", "Close", CSMLog.eMVerbosity.M_error, ex.ToString());
            }
        }

        private Task RegisterServerAndClient()
        {
            //We are on the UI thread. Grab a reference to the Dispatcher
            var context = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                try
                {
                    var fe = Main.ExecutionEngineFactory;
                   
                    var ds = new DataService(context, Main.ScriptCache, ExecutionEngine);

                    _host = DataServerHostFactory.Create(ds);
                    _host.Faulted += Host_Faulted;

                    _client = new DataServiceClient(_host.Description.Endpoints.First().Address.Uri);
                    _client.Open();
                    _client.Load();
                }
                catch (AddressAlreadyInUseException)
                {
                    //Another Sophis has already assumed the role of server. Sink the exception.
                    CSMLog.Write("Main", "EntryPoint", CSMLog.eMVerbosity.M_error, "Another instance is already listening and acting as the FusionLink Server");
                }
            });
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            CSMLog.Write("Main", "Host_Faulted", CSMLog.eMVerbosity.M_error, "The FusionInvest host has faulted.");
        }

        private static void RegisterExtensionLocations()
        {
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            path = SetPythonLocation(path);

            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
        }

        private static string SetPythonLocation(string path)
        {
            string pythonLocation = "";
            CSMConfigurationFile.getEntryValue("FusionScript", "PythonLocation", ref pythonLocation, "");

            var location = Environment.ExpandEnvironmentVariables(pythonLocation);
            location = location.Replace("%APPBASE%", AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            Environment.SetEnvironmentVariable("PYTHONHOME", location, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH ", Path.Combine(location, @"\Lib"), EnvironmentVariableTarget.Process);

            return location + @";" + path;
        }

        private static void RegisterScenarios()
        {
            CSMScenario.Register("OnLoadTriggerScenario", new AfterInitiationTriggerScenario());
        }

        private static void InitialiseCache()
        {
            ScriptCache = new ScriptCache(new FusionDb());
            ScriptCache.Load();
        }

        private void InitialiseMenu()
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(new Action(() =>
            {
                FusionScriptCommand.Register(_host.SingletonInstance as DataService, Main.ScriptCache, Main._editorManager);

                _commandManager = new FusionScriptCommandManager(ScriptCache, ExecutionEngineFactory);
                _commandManager.Initialize();

                var ribbon = RibbonBuilder.Instance.GetRibbon();
                RibbonBuilder.Instance.BuildRibbon(ribbon);

            }), DispatcherPriority.Normal);
        }

        private void ExecuteScriptOnloadTriggers()
        {
            //Kick off each script
            foreach (var script in Main.ScriptCache.GetAll())
            {
                if (script.Triggers.Any(x => x.Trigger == Model.Trigger.Load))
                {
                    var fe = new ExecutionService();
                    fe.ExecuteScript(script, Model.Trigger.Load);

                    continue;
                }
            }
        }
    }
}
