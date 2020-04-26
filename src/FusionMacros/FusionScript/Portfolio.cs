using System;
using System.Collections;
using System.Collections.Generic;
using sophis.misc;
using sophis.portfolio;
using sophis.scenario;
using sophis.utils;

namespace FusionScript
{
    ///<summary>
    ///Portfolio
    ///</summary>
    ///<example>
    ///import FusionMacro as fm
    ///p = fm.Portfolio(14812)
    ///print(p.Name)
    ///def PrintChildren(portfolio):
    ///    for x in portfolio.Children:
    ///        print((x.Level* ' ') + x.Name)
    ///        PrintChildren(x)
    ///PrintChildren(p)
    ///</example>
    public class Portfolio
    {
        /// <summary>
        /// Creates a portfolio with the given id
        /// </summary>
        /// <param name="id">The Folio Id</param>
        public Portfolio(int id)
        {
            Id = id;
        }

        /// <summary>
        /// The Folio Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The name of the portfolio
        /// </summary>
        public string Name
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                using var name = new CMString();
                p.GetName(name);
                return name.StringValue;
            }
        }

        /// <summary>
        /// The full name of the portfolio
        /// </summary>
        public string FullName
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                using var name = new CMString();
                p.GetFullName(name);
                return name.StringValue;  
            }
        }

        /// <summary>
        /// Whether the portfolio Is Locked
        /// </summary>
        public bool IsLocked
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                return p.IsLocked();
            }
        }

        /// <summary>
        /// Whether the portfolio marked as closed
        /// </summary>
        public bool IsClosed
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                return p.IsMarkedAsClosed();
            }
        }

        /// <summary>
        /// The parent portfolio
        /// </summary>
        public Portfolio Parent
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                var id = p.GetParentCode();
                return new Portfolio(id);
            }
        }

        /// <summary>
        /// The currency of the portfolio
        /// </summary>
        public Currency Currency
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                var id = p.GetCurrency();
                return new Currency(id);
            }
        }

        /// <summary>
        /// The entity of the portfolio
        /// </summary>
        public ThirdParty Entity
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                var id = p.GetEntity();
                return new ThirdParty(id);
            }
        }

        /// <summary>
        /// The level of the portfolio
        /// </summary>
        public int Level
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                return p.GetLevel();
            }
        }

        /// <summary>
        /// The comment
        /// </summary>
        public string Comment
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                using var comment = p.GetComment();
                return comment.StringValue;
            }
        }

        /// <summary>
        /// The Portfolio children
        /// </summary>
        public IEnumerable<Portfolio> Children
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);

                var childrenCount = p.GetSiblingCount();
                for(var i = 0; i < childrenCount; i++)
                {
                    var portfolio = p.GetNthSibling(i);
                    yield return new Portfolio(portfolio.GetCode());
                }
            }
        }

        /// <summary>
        /// Has an F8 being performed on the portfolio
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                using var p = CSMPortfolio.GetCSRPortfolio(Id);
                return p.IsLoaded();
            }
        }

        /// <summary>
        /// Perform an F8 on the portfolio
        /// </summary>
        public void Load()
        {
            using var p = CSMPortfolio.GetCSRPortfolio(Id);
            p.Load();
        }

        /// <summary>
        /// Perform an F9 on the portfolio
        /// </summary>
        public void Compute()
        {
            using var p = CSMPortfolio.GetCSRPortfolio(Id);
            p.Compute();
        }

        public object this[string name]
        {
            get 
            {
                return GetColumnValue(name);
            }
        }

        /// <summary>
        /// Gets a Portfolio Column value on the portfolio
        /// </summary>
        public object GetColumnValue(string name)
        {
            var cellValue = new SSMCellValue();
            using var gMain = sophis.globals.CSMExtraction.gMain();
            using var cellStyle = new SSMCellStyle();
            using var p = CSMPortfolioColumn.GetCSRPortfolioColumn(name);

            if(p is null)
                return $"'{name}' cannot be found.";
            
            p.GetPortfolioCell(Id, Id, gMain, ref cellValue, cellStyle, false);

            return DataTypeExtensions.ExtractValueFromSophisCell(cellValue, cellStyle);
        }

        /// <summary>
        /// Gets the portfolio positions
        /// </summary>
        public IEnumerable<Position> Positions
        {
            get
            {
                var results = new List<Position>();

                using var portfolio = CSMPortfolio.GetCSRPortfolio(Id);

                if (portfolio is object)
                {
                    if (!portfolio.IsLoaded())
                    {
                        throw new ApplicationException($"Porfolio not loaded {Id}");
                    }

                    GetPositionsFromPortfolio(portfolio, results);

                    var allChildren = new ArrayList();
                    portfolio.GetChildren(allChildren);

                    for (int i = 0; i < allChildren.Count; i++)
                    {
                        if (allChildren[i] is CSMPortfolio current)
                        {
                            GetPositionsFromPortfolio(current, results);
                        }
                    }

                    return results;
                }
                else
                {
                    throw new ApplicationException("PortfolioNotFound");
                }
            }
        }

        private void GetPositionsFromPortfolio(CSMPortfolio portfolio, List<Position> results)
        {
            int positionCount = portfolio.GetTreeViewPositionCount();
            for (int i = 0; i < positionCount; i++)
            {
                using var position = portfolio.GetNthTreeViewPosition(i);
                results.Add(new Position(position.GetIdentifier()));
                break;
            }
        }
    }
}
