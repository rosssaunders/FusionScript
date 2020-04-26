using System;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.Repository;
using sophis.utils;

namespace RxdSolutions.FusionScript.Runtimes
{
    public class ExecutionService
    {
        public ExecutionResults ExecuteScript(ScriptModel function)
        {
            IExecutionEngine executionEngine;
            executionEngine = GetExecutionEngine(function);

            var result = new ExecutionResults();
            result.StartTime = DateTime.UtcNow;

            try
            {
                result.Status = ExecutionStatus.Success; 
                result.Results = executionEngine.ExecuteScript(function.Script);
            }
            catch (Exception ex)
            {
                CSMLog.Write("ExecuteScript", nameof(ExecutionService), CSMLog.eMVerbosity.M_error, ex.ToString());

                result.Status = ExecutionStatus.Error;
                result.Results = ex.ToString();
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
            }

            return result;
        }

        public ExecutionResults ExecuteScript(ScriptModel function, Trigger trigger)
        {
            var result = ExecuteScript(function);

            SaveResultsToRepository(function, trigger, result.StartTime, result.EndTime, result.Results, result.Status);

            return result;
        }

        private static IExecutionEngine GetExecutionEngine(ScriptModel function)
        {
            IExecutionEngine executionEngine;
            try
            {
                //Execute the script once only
                //Load the execution to the database
                executionEngine = Main.ExecutionEngineFactory.GetExecutionEngine(function.Language);
            }
            catch (Exception ex)
            {
                CSMLog.Write("ExecuteScript", nameof(ExecutionService), CSMLog.eMVerbosity.M_error, ex.ToString());
                throw ex;
            }

            return executionEngine;
        }

        private static void SaveResultsToRepository(ScriptModel function, Trigger trigger, DateTime st, DateTime et, string results, ExecutionStatus status)
        {
            try
            {
                FusionDb.SaveScriptExecution(function.Id, trigger, st, et, status, results);
            }
            catch (Exception ex)
            {
                CSMLog.Write("SaveResultsToRepository", nameof(ExecutionService), CSMLog.eMVerbosity.M_error, ex.ToString());
                throw ex;
            }
        }
    }
}
