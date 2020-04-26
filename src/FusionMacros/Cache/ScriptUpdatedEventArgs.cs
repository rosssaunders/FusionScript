using System;

namespace RxdSolutions.FusionScript.Model
{
    public class ScriptUpdatedEventArgs : EventArgs
    {
        public int Id { get; }

        public ScriptModel Model { get; }

        public ScriptUpdatedEventArgs(int id, ScriptModel model)
        {
            Id = id;
            Model = model;
        }
    }
}
