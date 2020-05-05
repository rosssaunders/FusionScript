from System.Data import *

dt = DataTable("NetReturns")
dt.Columns.Add("Fund")
row = dt.NewRow()
row["Fund"] = "TEST"
dt.Rows.Add(row)

#Add to the main dataSet
dataSet.Tables.Add(dt)

