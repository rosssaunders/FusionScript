import clr
import FusionScript as fm
import random
from datetime import datetime

def PortfolioCellHandler(id): 
    value = random.random()
    result = fm.CellResult(value) 
    return result

def PositionCellHandler(id):
    position = fm.Position(id)
    instrumentType = position.GetColumnValue('Instrument Type')
    if(instrumentType == 'Forex'):
        return fm.CellResult(position.GetColumnValue('Diff. Result curr. global (D-1)')) 
    else:
        return fm.CellResult(random.random()) 

def UnderlyingCellHandler(id): 
    value = random.random()
    result = fm.CellResult(value) 
    return result

p = fm.PortfolioColumn('Random 17')
p.Group = 'Python'
p.Portfolio = fm.PortfolioCellValueHandler(PortfolioCellHandler)
p.Position = fm.PositionCellValueHandler(PositionCellHandler)
p.Underlying = fm.UnderlyingCellValueHandler(PositionCellHandler)
p.Register()