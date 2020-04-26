using CommandLine;

namespace RxdSolutions.FusionScript
{
    public class Options
    {
        [Option('s', "server", Required = false, HelpText = "The connection string to the server.")]
        public string Server { get; set; }

        [Option("id", Required = false, HelpText = "The Id of the macro to edit.")]
        public int ScriptId { get; set; }

        [Option("clone", Required = false, HelpText = "Whether to clone an existing macro.")]
        public bool Clone { get; set; }

        [Option("shutdown", Required = false, HelpText = "Whether to close the existing editors. Called when Sophis client is closed.")]
        public bool Shutdown { get; set; }
    }
}
