# ClaimsPaymentAccumulator
Custom Claims Payment Reader per Origin Year + Accumulated Values for every Development Year

Input : Incremental Claims Data
Product, Origin Year, Development Year, Incremental Value
Comp, 1992, 1992, 110.0
Comp, 1992, 1993, 170.0
Comp, 1993, 1993, 200.0
Non-Comp, 1990, 1990, 45.2
Non-Comp, 1990, 1991, 64.8
Non-Comp, 1990, 1993, 37.0
Non-Comp, 1991, 1991, 50.0
Non-Comp, 1991, 1992, 75.0
Non-Comp, 1991, 1993, 25.0
Non-Comp, 1992, 1992, 55.0
Non-Comp, 1992, 1993, 85.0
Non-Comp, 1993, 1993, 100.0


Output : Cumulative Claims Data
1990, 4
Comp, 0, 0, 0, 0, 0, 0, 0, 110, 280, 200
Non-Comp, 45.2, 110, 110, 147, 50, 125, 150, 55, 140, 100

