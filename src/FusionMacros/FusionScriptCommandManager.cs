using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Runtimes;
using RxdSolutions.FusionScript.Security;
using Sophis.Windows;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionScript
{
    internal class FusionScriptCommandManager
    {
        private readonly ScriptCache _cache;
        private readonly ExecutionEngineFactory _executionService;

        private readonly Dictionary<int, RibbonCommand> _commands = new Dictionary<int, RibbonCommand>();

        public FusionScriptCommandManager(ScriptCache cache, ExecutionEngineFactory executionService) 
        {
            _cache = cache;
            _executionService = executionService;
        }

        public void Initialize()
        {
            foreach (var script in _cache.GetAll())
            {
                CreateCommand(script);
            }

            _cache.ScriptChanged += Cache_MacroChanged;
            _cache.ScriptCreated += Cache_MacroChanged;
            _cache.ScriptDeleted += Cache_MacroDeleted;
        }

        private void CreateCommand(ScriptModel script)
        {
            var scriptCommand = new RibbonCommand("Execute " + script.Name, $"FusionScript_{script.Id}", typeof(RibbonCommand), script.Icon);

            RibbonCommands.RegisterDynamic(scriptCommand);
            SophisApplication.MainCommandTarget.CommandBindings.Add(new CommandBinding(scriptCommand, new ExecutedRoutedEventHandler(Handler), new CanExecuteRoutedEventHandler(CanExecuteHandler)));

            _commands.Add(script.Id, scriptCommand);
        }

        private void Cache_MacroChanged(object sender, ScriptUpdatedEventArgs e)
        {
            if(!_commands.ContainsKey(e.Id))
            {
                CreateCommand(e.Model);
            }
        }

        private void Cache_MacroDeleted(object sender, ScriptUpdatedEventArgs e)
        {
            if (_commands.ContainsKey(e.Id))
            {
                var existingCommand = _commands[e.Id];

                _commands.Remove(e.Id);

                RibbonCommands.Unregister(existingCommand);
            }
        }

        private void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = UserRights.CanExecute() || UserRights.CanManagePrivate() || UserRights.CanManageFirm();
        }

        private void Handler(object sender, ExecutedRoutedEventArgs e)
        {
            if (UserRights.CanExecute() || UserRights.CanManagePrivate() || UserRights.CanManageFirm())
            {
                var x = e.Command as RibbonCommand;
                var id = x.Name.Split('_')[1];
                var idInt = int.Parse(id);

                var script = _cache.GetAll().Single(x => x.Id == idInt);

                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var pe = _executionService.GetExecutionEngine(script.Language);
                    pe.ExecuteScript(script.Script);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
    }
}
