import clr
import sophis.scenario as scn
import sophis.market_data as md
import sophis.instrument as inst
import FusionMacro as fm

spx = inst.CSMInstrument.GetInstance(67606769)

xyz = fm.Instrument(67606769)
print(xyz.Name)

vol = spx.GetVolatilitySurface()
atm = vol.GetAtmVolatilityCurve(1, False)

atmPointCount = atm.GetPointCount()
print(atmPointCount)

smiles = atm.GetSmiles()
smileCount = smiles.GetSmileCount()

print(smileCount)

for i in range(smileCount):
    nthSmile = smiles.GetNthSmile(i)
    maturity = nthSmile.GetGoverningPoint().GetMaturity();
    moneynessLevel = vol.GetMoneynessLevel(maturity);
    print(maturity)
    print(moneynessLevel)

    for j in range(nthSmile.GetPointCount()):
        ssmsmilePoint = md.SSMSmilePoint();
        nthSmile.GetNthPoint(j, ssmsmilePoint);
        num = ssmsmilePoint.fVolatility;
        print(num)

