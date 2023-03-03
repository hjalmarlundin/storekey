namespace CampaignCalculatorApp;

using System;
using System.Linq;

public class ComboCalculator : IComboCalculator
{
    public IEnumerable<Product> ProcessCombinationDiscounts(List<ComboProduct> products)
    {
        var processedProducts = new List<Product>();

        // Process all items with combo category until empty
        while (products.Any())
        {
            var item = products[0];
            products.Remove(item);
            var comboItem = products.Find(x => x.ComboCategory == item.ComboCategory);

            // There is only one item in this category, do not give any discount
            if (comboItem == null)
            {
                processedProducts.Add(item with { CalculatedPrice = item.OriginalPrice });
            }

            // There is at least one more item, give discount
            else
            {
                processedProducts.Add(item with { CalculatedPrice = item.ComboPrice });
                processedProducts.Add(comboItem with { CalculatedPrice = comboItem.ComboPrice });
                products.Remove(comboItem);
            }
        }

        return processedProducts;
    }
}
