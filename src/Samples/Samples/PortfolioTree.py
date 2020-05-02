import FusionScript as fm
p = fm.Portfolio(14812)
print (p.Name)

def PrintChildren(portfolio):
    for x in portfolio.Children:
        print((x.Level * ' ') + x.Name)
        PrintChildren(x)

PrintChildren(p)
