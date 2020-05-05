using System;
using sophis.portfolio;
using sophis.scenario;
using System.Data;

namespace FusionScript
{
    /*
     * 
     * var x = 5;
            var y = 10;
            var z = x + y;

     * 
     * CSMScenario managedScenario = Utilities.GetManagedScenario("Param VaR And TE Compliance");
				base.fParam;
				managedScenario.SetBatchMode("Param VaR And TE Compliance", "729 10093 1", 0);
				managedScenario.Run();
				DataSet dataSet = new DataSet();
				managedScenario.ExportResult(dataSet);
				DataTable source = dataSet.Tables["varFolderResults"];



        CSMScenario csmscenario = CSMScenario.CreateFromPrototype(inParam);
			csmscenario.SetBatchMode(name, param, csmday.toLong());
			try
			{
				csmscenario.Initialise();
				csmscenario.Run();
				csmscenario.Done();
				CSMReportStrategy reportStrategy = csmscenario.GetReportStrategy();
				if (reportStrategy != null)
				{
					reportStrategy.EndResult(csmscenario);
				}
				csmscenario.AfterDone();
				M_SQLQuery.Commit();
			}
			catch (Exception ex)
			{
				M_SQLQuery.RollBack();
				logger.log(Severity.system_error, "Error while processing scenario : " + ex.Message);
				if (flag)
				{
					throw ex;
				}
			}
			logger.end();



     */


    //public class CustomScenario
    //{
    //    public void Register()
    //    {
    //        var scn = new CSMScenario2();
    //        CSMScenario.Register("Test1", scn);
    //    }

    //    public void X()
    //    {
    //        var vol = CSMVolatility.GetCSRVolatility(12345, 123123);


    //    }
    //}


    public class Scenario
    {
        public Scenario(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Run()
        {
            var csmscenario = CSMScenario.CreateFromPrototype(Name);

            if(csmscenario is null)
            {
                throw new ApplicationException("Unknown scenario");
            }

            csmscenario.SetBatchMode(Name, null, CSMPortfolio.GetPortfolioDate());
            csmscenario.Initialise();
            csmscenario.Run();
            csmscenario.Done();
            CSMReportStrategy reportStrategy = csmscenario.GetReportStrategy();
            if (reportStrategy != null)
            {
                reportStrategy.EndResult(csmscenario);
            }

            var dataSet = new DataSet();
            csmscenario.ExportResult(dataSet);

            csmscenario.AfterDone();
        }
    }
}
