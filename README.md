# Calculate campaign price

``` dotnet test ``` or build docker file to validate functionality and run tests.
CampaignCalculatorService.cs takes a array in EAN numbers as input and returns a result with the total price for said products and a list containing which products was used in the calculation.

Assumptions not specified in assignment:
* There exist items without combo campaign items or volume campaign items (Ordinary items).
* There can be many diffrent combo campaign categories.
* Two combo campaign item of the same EAN receives combo campaign discount.
* Non existing product in "database" will cause throwing exception.

