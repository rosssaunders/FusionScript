using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.portfolio;

namespace FusionScript.Column
{

    /// <summary>
    /// Slimed down version of GetPositionCell with just the Position details
    /// </summary>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate CellResult PositionCellValueHandler(Position position);

    /// <summary>
    /// Gives the implementer full access to all the parameters passed to the GetPositionCell function
    /// </summary>
    /// <remarks>
    /// CSMPosition position, int activePortfolioCode, int portfolioCode, CSMExtraction extraction, int underlyingCode, int instrumentCode
    /// </remarks>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate CellResult PositionCellValueHandlerEx(Position position, Portfolio activePortfolio, Portfolio portfolio, Extraction extraction, Instrument underlying, Instrument instrument);

    /// <summary>
    /// Slimed down version of GetPortfolioCell with just the Position details
    /// </summary>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate CellResult PortfolioCellValueHandler(Portfolio portfolio);

    /// <summary>
    /// Gives the implementer full access to all the parameters passed to the GetPortfolioCell function
    /// </summary>
    /// <remarks>
    /// int activePortfolioCode, int portfolioCode, CSMExtraction extraction
    /// </remarks>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate CellResult PortfolioCellValueHandlerEx(Portfolio activePortfolio, Portfolio portfolio, Extraction extraction);


    /// <summary>
    /// Slimed down version of GetPortfolioCell with just the Position details
    /// </summary>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate CellResult UnderlyingCellValueHandler(Instrument underlying);

    /// <summary>
    /// Gives the implementer full access to all the parameters passed to the GetUnderlyingCell function
    /// </summary>
    /// <remarks>
    /// int activePortfolioCode, int portfolioCode, CSMExtraction extraction, int underlyingCode
    /// </remarks>
    /// <returns>The CellResult containing the cell values and metadata</returns>
    public delegate CellResult UnderlyingCellValueHandlerEx(Portfolio activePortfolio, Portfolio portfolio, Extraction extraction, Instrument underlying);
}
