import tempfile
import os
import pandas as pd
import math
import webbrowser
import numpy as np
import matplotlib as mpl
mpl.use('agg')

import base64
from io import BytesIO
import matplotlib.pyplot as plt
from scipy.integrate import quad
from scipy.optimize import brentq
from scipy.interpolate import interp2d
from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm

from sophis.guicommon.data.volatility import *
import FusionScript as fm
from sophis.finance import *
from sophis.instrument import *
from sophis.utils import *

df = pd.DataFrame()

sp = fm.Application.SelectedPositions

i = fm.Position(sp[0]).Instrument
opt = i.Option.GetOption()
fxi = fm.Instrument(opt.GetUnderlyingCode()).ForexSpot.GetForexSpot()
opt.Dispose()

ccy1 = fm.Currency(fxi.GetForex1())
ccy2 = fm.Currency(fxi.GetForex2())
inverse = fm.Instrument(ccy2.IsoCode + "/" + ccy1.IsoCode)

if(ccy1.IsoCode == "USD"):
    ccy1 = inverse.Id
    ccy2 = i.Option.GetOption().GetUnderlyingCode()
else:
    ccy1 = i.Option.GetOption().GetUnderlyingCode()
    ccy2 = inverse.Id

vd = VolatilityMatrixData(ccy1, ccy2, 1)
vd.LoadVolatility()
    
i = 0
for x in vd.mTableCallAndPut.Rows:
    maturityValue = str(x['Maturity'])
    if("Strike" in maturityValue):
        continue

    maturity = maturityValue = str(x['MaturityName'])
    atmVol = float(x["ATM Vol"])
    j = 0
    for c in vd.mTableCallAndPut.Columns:

        vol = 0
        val = x[c.ColumnName]

        if("Delta" in c.ColumnName):
            vol = float(val) + atmVol
        elif("ATM Vol" in c.ColumnName):
            vol = atmVol
        else:
            vol = str(val)

        df.at[i, c.ColumnName] = vol

        j += 1

    i += 1

data = pd.melt(df, id_vars=['MaturityName'], value_vars=['10 Delta Put', '15 Delta Put', '25 Delta Put', '35 Delta Put', 'ATM Vol', '35 Delta Call', '25 Delta Call', '15 Delta Call', '10 Delta Call'])

def ttm(tenor_period):
    if 'w' in tenor_period:
        return 7
    elif 'm' in tenor_period:
        return 30
    elif 'y' in tenor_period:
        return 360
    else:
        return 30

def strike(row):
    if("Call" in row.variable):
        return 100 - int(row.variable[0:2])
    elif("Put" in row.variable):
        return int(row.variable[0:2])
    elif("ATM Vol" in row.variable):
        return 50
    else:
        return 0

data['TenorValue'] = data.apply(lambda row: int(row.MaturityName[0:1]), axis=1)
data['TenorPeriod'] = data.apply(lambda row: row.MaturityName[1:], axis=1)
data['TTM'] = data.apply(lambda row: row.TenorValue * ttm(row.TenorPeriod), axis=1)

data['Strike'] = data.apply(lambda row: strike(row), axis=1)
data['ImpliedVol'] = data.apply(lambda row: float(row.value), axis=1)

ttm = data['TTM'].tolist()
strikes = data['Strike'].tolist()
imp_vol = data['ImpliedVol'].tolist()

f = interp2d(strikes,ttm,imp_vol, kind='linear')
plot_strikes = np.linspace(data['Strike'].min(), data['Strike'].max(),25)
plot_ttm = np.linspace(0, data['TTM'].max(),25)
fig = plt.figure(figsize=(10,5))
ax = fig.gca(projection='3d')
X, Y = np.meshgrid(plot_strikes, plot_ttm)
Z = np.array([f(x,y) for xr, yr in zip(X, Y) for x, y in zip(xr,yr) ]).reshape(len(X), len(X[0]))
surf = ax.plot_surface(X, Y, Z, rstride=1, cstride=1, cmap=cm.coolwarm,linewidth=0.1)
ax.set_xlabel('Strike %ATM')
ax.set_ylabel('Time-to-Maturity (d)')
ax.set_zlabel('Implied Volatility')
fig.colorbar(surf, shrink=0.5, aspect=5)#

def savePlotAsBase64(plt):
    image = BytesIO()
    plt.savefig(image, format='png')
    image.seek(0)
    return base64.b64encode(image.read()).decode()

tmp = tempfile.NamedTemporaryFile(delete=False)
path = tmp.name + '.html'
f = open(path, 'w')
f.write('<HTML><BODY>')
f.write('<img src="data:image/png;base64, ' + savePlotAsBase64(plt) + '" />')
f.write(df.to_html())
f.write('</BODY></HTML>')
f.close()

webbrowser.open('file://' + path)
