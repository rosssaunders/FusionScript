////using System.Data.Entity;
////using Oracle.DataAccess.Client;

//using System.Collections.Generic;
//using Oracle.DataAccess.Client;
//using SqlKata;
//using SqlKata.Execution;
//using SqlKata.Compilers;
//using System.Linq;

//namespace FusionScript
//{
//    public class Database
//    {
//        private readonly OracleConnection _connection;
//        private readonly QueryFactory _db;

//        public Database(OracleConnection connection)
//        {
//            _connection = connection;

//            var compiler = new OracleCompiler();
//            _db = new QueryFactory(connection, compiler);
//        }

//        public IList<PortfolioX> GetFolios(IQueryable<PortfolioX> filter)
//        {
//            var results = _db.Query("FOLIO")
                                
//                                .Where(filter)
//                                .Get<PortfolioX>();

//            return results.ToList();   
//        }
//    }

//    public class PortfolioX
//    {
//        public int IDENT { get; set; }

//        public string NAME { get; set; }
//    }
//}
