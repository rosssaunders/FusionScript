using System;

namespace RxdSolutions.FusionScript.Runtimes
{
    [Serializable]
    public class ExecutionException : Exception
    {
        public ExecutionException()
        {
        }

        public ExecutionException(string message) : base(message)
        {
        }

        public ExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExecutionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
