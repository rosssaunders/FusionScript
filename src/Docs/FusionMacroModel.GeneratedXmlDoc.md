# FusionMacroModel #

## Type FusionMacro.Application

 Application 



---
## Type FusionMacro.Portfolio

 Portfolio 

##### Example:  import FusionMacro as fm p = fm.Portfolio(14812) print(p.Name) def PrintChildren(portfolio): for x in portfolio.Children: print((x.Level* ' ') + x.Name) PrintChildren(x) PrintChildren(p) 



---
#### Method FusionMacro.Portfolio.#ctor(System.Int32)

 Creates a portfolio with the given id 

|Name | Description |
|-----|------|
|id: |The Folio Id|


---
#### Property FusionMacro.Portfolio.Id

 The Folio Id 



---
#### Property FusionMacro.Portfolio.Name

 The name of the portfolio 



---
#### Property FusionMacro.Portfolio.FullName

 The full name of the portfolio 



---
#### Property FusionMacro.Portfolio.IsLocked

 Whether the portfolio Is Locked 



---
#### Property FusionMacro.Portfolio.IsClosed

 Whether the portfolio marked as closed 



---
#### Property FusionMacro.Portfolio.Parent

 The parent portfolio 



---
#### Property FusionMacro.Portfolio.Currency

 The currency of the portfolio 



---
#### Property FusionMacro.Portfolio.Entity

 The entity of the portfolio 



---
#### Property FusionMacro.Portfolio.Level

 The level of the portfolio 



---
#### Property FusionMacro.Portfolio.Comment

 The comment 



---
#### Property FusionMacro.Portfolio.Children

 The Portfolio children 



---
#### Property FusionMacro.Portfolio.IsLoaded

 Has an F8 being performed on the portfolio 



---
#### Method FusionMacro.Portfolio.Load

 Perform an F8 on the portfolio 



---
#### Method FusionMacro.Portfolio.Compute

 Perform an F9 on the portfolio 



---
#### Method FusionMacro.Portfolio.GetColumnValue(System.String)

 Gets a Portfolio Column value on the portfolio 



---
#### Property FusionMacro.Portfolio.Positions

 Gets the portfolio positions 



---


