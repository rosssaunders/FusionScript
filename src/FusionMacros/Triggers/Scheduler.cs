using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Runtimes;
using sophis.utils;

namespace RxdSolutions.FusionScript.Triggers
{
    internal class Scheduler
    {
        private readonly ScriptCache _cache;
        private readonly Dispatcher _dispatcher;
        private readonly ExecutionService _executionEngine;
        private readonly CancellationTokenSource _source;
        private readonly CancellationToken _token;

        private Task _check;
        private DateTime _lastCheck;

        public Scheduler(ScriptCache cache, Dispatcher dispatcher, ExecutionService executionEngine)
        {
            _source = new CancellationTokenSource();
            _token = _source.Token;
            _cache = cache;
            _dispatcher = dispatcher;
            _executionEngine = executionEngine;
            _lastCheck = DateTime.Now;
        }

        public void Start()
        {
            if (_check == null)
            {
                _check = Task.Run(new Action(() =>
                {
                    while (!_token.IsCancellationRequested)
                    {
                        var localLastCheck = _lastCheck;
                        _lastCheck = DateTime.Now;

                        foreach (var function in _cache.GetAll())
                        {
                            //Has any triggers fired since our last check?
                            foreach(var trigger in function.Triggers)
                            {
                                if(trigger.Trigger == Trigger.Schedule)
                                {
                                    if(localLastCheck.TimeOfDay < trigger.Time.TimeOfDay && trigger.Time.TimeOfDay <= DateTime.Now.TimeOfDay) 
                                    {
                                        _dispatcher.InvokeAsync(new Action(() =>
                                        {
                                            try
                                            {
                                                _executionEngine.ExecuteScript(function, Trigger.Schedule);
                                            }
                                            catch(Exception ex)
                                            {
                                                CSMLog.Write(nameof(Scheduler), nameof(Start), CSMLog.eMVerbosity.M_error, ex.ToString());
                                            }
                                            
                                        }));
                                    }
                                }
                            }
                        }

                        Thread.Sleep(1000);
                    }

                }), _token);
            }
        }

        public void Stop()
        {
            _source.Cancel();
        }
    }
}
