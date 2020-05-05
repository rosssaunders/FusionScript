import base64
from io import BytesIO
import tempfile
import random
import os
import win32com.client as win32
import matplotlib
matplotlib.use('agg')
import matplotlib.pyplot as plt 
from FusionScript import *
from collections import defaultdict
from collections import namedtuple

def get_loaded_portfolio(id):
    portfolio = Portfolio(27557)
    if portfolio.IsLoaded is False:
        portfolio.Load()

    portfolio.Compute()
    return portfolio

def get_breakdown():

    portfolio = get_loaded_portfolio(27557)

    pnlBySector = defaultdict(float)

    for position in portfolio.Positions:
        sector = position.GetColumnValue("Ranking Systematic Sector")
        dailyPnL = position.GetColumnValue("Diff. Result curr. global (D-1)")

        if sector == '':
            continue

        if dailyPnL is None:
            dailyPnL = 0

        pnlBySector[sector] += dailyPnL * random.random()

    return pnlBySector

def create_graph():
    sectorPnL = get_breakdown()

    x = list(sectorPnL.keys())
    y = list(sectorPnL.values())
    x_pos = [i for i, _ in enumerate(x)]

    plt.clf()

    fig, ax = plt.subplots(figsize=(10, 5))
    plt.xticks(x_pos, x, alpha=1)

    ax.bar(x_pos, y, color=['darkblue' if x > 0 else 'red' for x in y]) 
    ax.set_xlabel('Sector', alpha=0.7) 
    ax.set_ylabel('Daily P&L', alpha=0.7) 
    ax.set_title ('Sector Breakdown') 

    #removing top and right borders
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    ax.grid(color='grey', linestyle='-', linewidth=0.25, alpha=0.5)

    image = BytesIO()
    plt.savefig(image, format='png')
    image.seek(0)
    return base64.b64encode(image.read()).decode()

def send_mail(body, attachments):
    outlook = win32.Dispatch('outlook.application')
    mail = outlook.CreateItem(0)
    mail.To = 'ross@rxdsolutions.co.uk'
    mail.Subject = 'Flash P&L'
    mail.HTMLBody = body

    for att in attachments:
        mail.Attachments.Add(att)

    mail.Display()

def get_flash_pnl(id):
    portfolio = get_loaded_portfolio(id)
    if portfolio.IsLoaded is False:
        portfolio.Load()

    portfolio.Compute()
    daily = portfolio.GetColumnValue("Diff. Result curr. global (D-1)")
    mtd = portfolio.GetColumnValue("Diff. Result curr. global (M-1)")
    qtd = portfolio.GetColumnValue("Diff. Result curr. global (Q-1)")
    ytd = portfolio.GetColumnValue("Diff. Result curr. global (Y-1)")

    daily = daily * random.random()
    mtd = mtd * random.random()
    qtd = qtd * random.random()
    ytd = ytd * 100 * random.random()

    return daily, mtd, qtd, ytd

def top_winners_losers(id):
    portfolio = get_loaded_portfolio(id)

    PositionWithPnL = namedtuple('PositionWithPnL', 'name pnl')
    results = [ PositionWithPnL(position.Instrument.Reference, position.GetColumnValue("Diff. Result curr. global (D-1)")) for position in portfolio.Positions]
    resultsSorted = sorted([x for x in results if x.pnl is not None], key=lambda x: x.pnl)
    return resultsSorted

def returns_table(funds):
        
    resultHTML = '<table><thead><tr><th>Fund</th><th>Daily</th><th>MTD</th><th>QTD</th><th>YTD</th></tr></thead><tbody>' 

    for fund in funds:
        daily, mtd, qtd, ytd = get_flash_pnl(fund)

        resultHTML += '<tr>'
        resultHTML += '<td>' + 'Alpha Edge' + '</td>'
        resultHTML += '<td>' + f'{daily:,.0f}' + '</td>'
        resultHTML += '<td>' + f'{mtd:,.0f}' + '</td>'
        resultHTML += '<td>' + f'{qtd:,.0f}' + '</td>'
        resultHTML += '<td>' + f'{ytd:,.0f}' + '</td>'
        resultHTML += '</tr>'

    resultHTML += '</tbody></table>'
    return resultHTML

funds = [27557]

html = returns_table(funds)

graph_as_base64 = create_graph()

html += '<p><img src="data:image/png;base64, ' + graph_as_base64 + '" /></p>'

print(html)

html += '<H1>Winners</H1>'

winners_table = '<table>'
positions = top_winners_losers(29937)
for pos in positions[:5]:
    winners_table += '<tr><td>' + str(pos.name) + '</td><td>' + f'{pos.pnl:,.0f}' + '</td></tr>'

winners_table += '</table>'

html += winners_table

html += '<H1>Losers</H1>'

losers_table = '<table>'
for pos in positions[-5:]:
    losers_table += '<tr><td>' + str(pos.name) + '</td><td>' + f'{pos.pnl:,.0f}' + '</td></tr>'

losers_table += '</table>'

html += losers_table

print(html)
#send_mail(html, None)