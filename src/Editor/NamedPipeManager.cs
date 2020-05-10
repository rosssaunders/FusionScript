using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace RxdSolutions.FusionScript
{
    /// <summary>
    /// A very simple Named Pipe Server implementation that makes it 
    /// easy to pass string messages between two applications.
    /// </summary>
    public class NamedPipeManager
    {
        private readonly string namedPipeName;

        public event EventHandler<string> ReceiveString;

        public const string EXIT_STRING = "__EXIT__";
        public const string SHUTDOWN_STRING = "__SHUTDOWN__";
        
        private bool _isRunning = false;
        private Thread Thread;

        public NamedPipeManager(string name)
        {
            namedPipeName = name;
        }

        /// <summary>
        /// Starts a new Pipe server on a new thread
        /// </summary>
        public void StartServer()
        {
            Thread = new Thread((pipeName) =>
            {
                _isRunning = true;

                Debugger.Break();

                while (true)
                {
                    string text;
                    using (var server = new NamedPipeServerStream(pipeName as string))
                    {
                        server.WaitForConnection();

                        using var reader = new StreamReader(server);

                        text = reader.ReadToEnd();
                    }

                    if (text == EXIT_STRING)
                        return;

                    OnReceiveString(text);

                    if (_isRunning == false)
                        return;
                }
            });

            Thread.Start(namedPipeName);
        }

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnReceiveString(string text)
        {
            ReceiveString?.Invoke(this, text);
        }

        /// <summary>
        /// Shuts down the pipe server
        /// </summary>
        public void StopServer()
        {
            _isRunning = false;
            Write(EXIT_STRING);
            Thread.Join(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Write a client message to the pipe
        /// </summary>
        /// <param name="text"></param>
        /// <param name="connectTimeout"></param>
        public bool Write(string text, int connectTimeout = 300)
        {
            using (var client = new NamedPipeClientStream(namedPipeName))
            {
                try
                {
                    client.Connect(connectTimeout);
                }
                catch(Exception ex)
                {
                    Debug.Print(ex.ToString());

                    return false;
                }

                if (!client.IsConnected)
                    return false;

                using var writer = new StreamWriter(client);

                writer.Write(text);
                writer.Flush();
            }
            return true;
        }
    }
}
