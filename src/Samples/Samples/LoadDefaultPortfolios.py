from FusionScript import *
portfoliosToLoad = [29937,29938,30499]

for folioId in portfoliosToLoad:

    print('Loading ' + str(folioId))

    p = Portfolio(folioId)

    if(p.IsLoaded == False):
        p.Load()

    p.Compute()
