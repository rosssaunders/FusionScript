from FusionMacro import *
portfoliosToLoad = [29937,29938,30499] #Equity Long/Short, Global Macro, Hedge book

for folioId in portfoliosToLoad:

    print('Loading ' + str(folioId))

    p = Portfolio(folioId)

    if(p.IsLoaded == False):
        p.Load()

    p.Compute()
