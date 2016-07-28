#### Irs
response field mapping:

|QDP|Qb|
|---|---|
|valuationDate|curveDate
|effectiveDate|curveValueDate
|IrsTrade TradeInfo.SwapDirection|direction|
|TradeInfo.StartDate|effectiveDate|
|Result.FairRate|fairRate|
|date of the first cashflow|firstSettlementDate|
|Ai|fixedAccrued|
|need to parse from TradeId|forwardEndTenors|
||forwardMatrix|
|need to parse from TradeId|forwardStartTenors|
|`new Term(tenor).Next(startDate.ToDate()).ToString()`|maturityDate|
|in floatingleg result, first unpaid `cashflow.CalculationDetails[0].FixingRate`|lastFixing|
|in floatingleg result, first unpaid `cashflow.CalculationDetails[0].FixingDate`|lastFixingDate|


