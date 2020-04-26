using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.portfolio;
using Sophis.Data.Utils;

namespace FusionMacro
{
    /// <summary>
    /// Application
    /// </summary>
    public static class Application
    {
        public static int PortfolioColumnRefreshVersion
        {
            get
            {
                return CSMPortfolioColumn.GetRefreshVersion();
            }
        }

        public static IEnumerable<int> SelectedPortfolios
        {
            get
            {
                IList<ITreeNode> selectedNodes = NavigationManager.Instance.SelectedNodes;

                var ids = new List<int>();
                if (selectedNodes is object && selectedNodes.Count > 0)
                {
                    foreach (var sn in selectedNodes)
                    {
                        if (sn is PortfolioNode p)
                        {
                            ids.Add(p.PortfolioIdent);
                        }
                    }
                }

                return ids;
            }
        }

        public static IEnumerable<int> SelectedPositions
        {
            get
            {
                IList<ITreeNode> selectedNodes = NavigationManager.Instance.SelectedNodes;

                var ids = new List<int>();
                if (selectedNodes is object && selectedNodes.Count > 0)
                {
                    foreach (var sn in selectedNodes)
                    {
                        if (sn is PositionNode p)
                        {
                            ids.Add(p.PositionId);
                        }
                    }
                }

                return ids;
            }
        }
    }
}
