using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class MacroUpdatedEventArgs
    {
        public int Id { get; internal set; }
        
        public ScriptModel Model { get; internal set; }
    }
}