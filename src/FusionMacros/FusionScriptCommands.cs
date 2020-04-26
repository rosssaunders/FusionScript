using System;
using System.Windows.Input;
using RxdSolutions.FusionScript.Interface;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.ViewModels;
using RxdSolutions.FusionScript.Views;
using Sophis.Windows;
using Sophis.Windows.Integration;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionScript
{
    internal class FusionScriptCommand : RibbonCommand
    {
        private readonly IDataService _service;
        private readonly ScriptCache _modelCache;
        private readonly EditorManager _dataEditorManager;

        public FusionScriptCommand(IDataService service, ScriptCache modelCache, EditorManager editorManager) : base("Display FusionScript", "FusionScript", typeof(FusionScriptCommand), "text_code_edit")
        {
            _service = service;
            _modelCache = modelCache;
            _dataEditorManager = editorManager;
            
            SophisApplication.MainCommandTarget.CommandBindings.Add(new CommandBinding(this, new ExecutedRoutedEventHandler(Handler), new CanExecuteRoutedEventHandler(CanExecuteHandler)));
        }

        public static void Register(IDataService service, ScriptCache modelCache, EditorManager editorManager)
        {
            var scriptCommand = new FusionScriptCommand(service, modelCache, editorManager);
            RibbonCommands.RegisterDynamic(scriptCommand);
        }

        private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Handler(object sender, ExecutedRoutedEventArgs e)
        {
            var wndKey = new WindowKey(4039, 0, 0);
            IntPtr activeWindow = WPFAdapter.Instance.GetActiveWindow(wndKey);
            if (IntPtr.Zero != activeWindow)
            {
                WPFAdapter.Instance.ActivateWindow(activeWindow);
            }
            else
            {
                var viewModel = new ScriptsViewModel(_service, _modelCache, _dataEditorManager);
                var fwkElement = new ScriptsView(viewModel);
                WPFAdapter.Instance.OpenWindow(fwkElement, "FusionScript", wndKey, false);
            }
        }
    }
}
