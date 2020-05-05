from sophis.instrument import *
from sophis.utils import *
from sophis.misc import *
from sophisTools import *
import clr
import datetime

def getString(x):
    y = CMString()
    x(y)
    return y.StringValue

def getDateTime(x):
    dt = CSMDay(x)
    return datetime.datetime(dt.fYear, dt.fMonth, dt.fDay)

def getLegType(leg):

    if(leg.GetTypeOf() == 0): #Floating
        floatingLeg = clr.Convert(leg, CSMFloatingLeg)
        return floatingLeg

    if(leg.GetTypeOf() == 1): #Fixed
        fixedLeg = clr.Convert(leg, CSMFixedLeg)
        return fixedLeg

instrument = CSMInstrument.GetInstance(67875062)

swap = clr.Convert(instrument, CSMSwap)

print('Swap Parameters')
print()

print(" CurrencyCode =", swap.GetCurrencyCode())
print(' Reference =', swap.GetReference())
print(' Name =', getString(swap.GetName))
print(" StartDate =", getDateTime(swap.GetStartDate()))
print(" EndDate =", getDateTime(swap.GetEndDate()))
print(" Notional =", swap.GetNotional())
print(" ModelName =", getString(swap.GetModelName))
print(" QuotationUnit =" , swap.GetAskQuotationType())

print()
print('Receiving leg payoff')
print()

#Receiving Leg object
receivingLeg = swap.GetLeg(0)

print(' ReceivingLegType =', receivingLeg.GetTypeOf())
print(' ReceivingLegCurrencyCode =', receivingLeg.GetCurrencyCode())
print(' ReceivingLegSettlementCurrencyType =', receivingLeg.GetSettlementCurrencyType())
print(' ReceivingLegSettlementCurrency =', receivingLeg.GetDeliveryCurrency())

#Fixed
if(receivingLeg.GetTypeOf() == 1):
    print()
    fixedLeg = getLegType(receivingLeg)
    print(' ReceivingLegFixedSettlementTiming =', fixedLeg.GetSettlementTiming())
    print(' ReceivingLegFixedCouponFrequency =', fixedLeg.GetPeriodicityType())
    print(' ReceivingLegFixedCouponBasis =', fixedLeg.GetDayCountBasisType())
    print(' ReceivingLegFixedCouponMode =', fixedLeg.GetYieldCalculationType())
    print(' ReceivingLegFixedRate =', fixedLeg.GetFixedRate())
    print(' ReceivingLegFixedAccrual =', fixedLeg.GetAccrualMode())
    
#Floating
if(receivingLeg.GetTypeOf() == 0):
    print()
    floatingLeg = getLegType(receivingLeg)
    print(' ReceivingLegFloatingSettlementTiming =', floatingLeg.GetSettlementTiming())
    print(' ReceivingLegFloatingCouponBasis =', floatingLeg.GetDayCountBasisType())
    print(' ReceivingLegFloatingCouponSpread =', floatingLeg.GetSpread())
    print(' ReceivingLegFloatingCouponCapitalization =', floatingLeg.GetCapitalization())
    print(' ReceivingLegFloatingResetIndexCode =', floatingLeg.GetFloatingRate())
    print(' ReceivingLegFloatingResetFrequency =', floatingLeg.GetPeriodicityType())
    print(' ReceivingLegFloatingResetTiming =', floatingLeg.GetFixingDateRef())
    print(' ReceivingLegFloatingResetMinimum =', floatingLeg.GetMinimum())
    print(' ReceivingLegFloatingResetMaximum =', floatingLeg.GetMaximum())
    print(' ReceivingLegFloatingResetRoundingMethod =', floatingLeg.GetRoundingMethodType())

print()
print('Paying leg payoff')
print()

payingLeg = swap.GetLeg(1)

print(' PayingLegType =', payingLeg.GetTypeOf())
print(' PayingLegCurrencyCode =', payingLeg.GetCurrencyCode())
print(' PayingLegSettlementCurrencyType =', payingLeg.GetSettlementCurrencyType())
print(' PayingLegSettlementCurrency =', payingLeg.GetDeliveryCurrency())

#Fixed
if(payingLeg.GetTypeOf() == 1):
    print()
    fixedLeg = getLegType(payingLeg)
    print(' PayingLegFixedSettlementTiming =', fixedLeg.GetSettlementTiming())
    print(' PayingLegFixedCouponFrequency =', fixedLeg.GetPeriodicityType())
    print(' PayingLegFixedCouponBasis =', fixedLeg.GetDayCountBasisType())
    print(' PayingLegFixedCouponMode =', fixedLeg.GetYieldCalculationType())
    print(' PayingLegFixedRate =', fixedLeg.GetFixedRate())
    print(' PayingLegFixedAccrual =', fixedLeg.GetAccrualMode())

#Floating
if(payingLeg.GetTypeOf() == 0):
    print()
    floatingLeg = getLegType(payingLeg)
    print(' PayingLegFloatingSettlementTiming =', floatingLeg.GetSettlementTiming())
    print(' PayingLegFloatingCouponBasis =', floatingLeg.GetDayCountBasisType())
    print(' PayingLegFloatingCouponSpread =', floatingLeg.GetSpread())
    print(' PayingLegFloatingCouponCapitalization =', floatingLeg.GetCapitalization())
    print(' PayingLegFloatingResetIndexCode =', floatingLeg.GetFloatingRate())
    print(' PayingLegFloatingResetFrequency =', floatingLeg.GetPeriodicityType())
    print(' PayingLegFloatingResetTiming =', floatingLeg.GetFixingDateRef())
    print(' PayingLegFloatingResetMinimum =', floatingLeg.GetMinimum())
    print(' PayingLegFloatingResetMaximum =', floatingLeg.GetMaximum())
    print(' PayingLegFloatingResetRoundingMethod =', floatingLeg.GetRoundingMethodType())

print()
print('Advanced')
print()

print(' CalculationAgentCode =', swap.GetCalculationAgent())
print(' Allotment =', swap.GetAllotment())
print(' PaymentGapType =', swap.GetPaymentGapType())
print(' Comment =', getString(swap.GetComment))

print()
print('Notional')
print()

print(' NotionalExchangeType =', swap.GetNotionalExchangeType())

swap.Dispose()
