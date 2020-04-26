using System;
using RxdSolutions.FusionScript.Model;

namespace RxdSolutions.FusionScript.Client
{
    public class ScriptUpdateEventArgs : EventArgs
    {
        public ScriptUpdateEventArgs(int id, ScriptModel model)
        {
            Id = id;
            Model = model;
        }

        public int Id { get; }

        public ScriptModel Model { get; }
    }
}
