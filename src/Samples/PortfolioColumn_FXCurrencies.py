import clr
import FusionScript as fm
import FusionScript.Column as col
from datetime import datetime

def PositionCellHandlerCcy1(position):
    p = position
    currency = 0
    if(p.Instrument.Type == 'E'):
        inst = p.Instrument.ForexSpot.GetForexSpot()
        currency = inst.GetForex1()
        inst.Dispose()
    elif(p.Instrument.Type == 'X'):
        inst = p.Instrument.ForexForward.GetForexForward()
        currency = inst.GetExpiryCurrency()
        inst.Dispose()
    elif(p.Instrument.Type == 'K'):
        inst = p.Instrument.ForexNonDeliverableForward.GetNDFForex()
        currency = inst.GetExpiryCurrency()
        inst.Dispose()
    elif(p.Instrument.Type == 'F'):
        inst = p.Instrument.Future.GetFuture()
        if (inst.GetUnderlyingNature() == 2):
            currency = inst.GetCurrency()
        inst.Dispose()

    if currency == 0:
        return col.CellResult('')
    else:
        cr = col.CellResult(fm.Currency(currency).IsoCode)
        cr.Currency = currency
        return cr

p = col.PortfolioColumn('Forex Currency 1')
p.Group = 'Python'
p.Position = col.PositionCellValueHandler(PositionCellHandlerCcy1)
p.Register()


def PositionCellHandlerCcy2(position):
    p = position
    currency = 0
    if(p.Instrument.Type == 'E'):
        inst = p.Instrument.ForexSpot.GetForexSpot()
        currency = inst.GetForex2()
        inst.Dispose()
    elif(p.Instrument.Type == 'X'):
        inst = p.Instrument.ForexForward.GetForexForward()
        currency = inst.GetCurrency()
        inst.Dispose()
    elif(p.Instrument.Type == 'K'):
        inst = p.Instrument.ForexNonDeliverableForward.GetNDFForex()
        currency = inst.GetCurrency()
        inst.Dispose()
    elif(p.Instrument.Type == 'F'):
        inst = p.Instrument.Future.GetFuture()
        if (inst.GetUnderlyingNature() == 2):
            currency = inst.GetUnderlyingCode()
        inst.Dispose()

    if currency == 0:
        return col.CellResult('')
    else:
        cr = col.CellResult(fm.Currency(currency).IsoCode)
        cr.Currency = currency
        return cr

p = col.PortfolioColumn('Forex Currency 2')
p.Group = 'Python'
p.Position = col.PositionCellValueHandler(PositionCellHandlerCcy2)
p.Register()