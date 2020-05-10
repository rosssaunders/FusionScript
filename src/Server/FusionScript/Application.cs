using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.portfolio;
using Sophis.Data.Utils;

namespace FusionScript
{
    /// <summary>
    /// Application
    /// </summary>
    public static class Application
    {
        static Application()
        {
            //Database = new Database(Sophis.DataAccess.DBContext.Connection);
        }

        //public static Database Database { get; private set; }

        public static int PortfolioColumnRefreshVersion
        {
            get
            {
                return CSMPortfolioColumn.GetRefreshVersion();
            }
        }

        public static IEnumerable<Portfolio> SelectedPortfolios
        {
            get
            {
                IList<ITreeNode> selectedNodes = NavigationManager.Instance.SelectedNodes;

                var ids = new List<Portfolio>();
                if (selectedNodes is object && selectedNodes.Count > 0)
                {
                    foreach (var sn in selectedNodes)
                    {
                        if (sn is PortfolioNode p)
                        {
                            ids.Add(new Portfolio(p.PortfolioIdent));
                        }
                    }
                }

                return ids;
            }
        }

        public static IEnumerable<Position> SelectedPositions
        {
            get
            {
                IList<ITreeNode> selectedNodes = NavigationManager.Instance.SelectedNodes;

                var ids = new List<Position>();
                if (selectedNodes is object && selectedNodes.Count > 0)
                {
                    foreach (var sn in selectedNodes)
                    {
                        if (sn is PositionNode p)
                        {
                            ids.Add(new Position(p.PositionId));
                        }
                    }
                }

                return ids;
            }
        }
    }
}
