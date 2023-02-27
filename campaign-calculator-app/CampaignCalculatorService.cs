namespace CampaignCalculatorApp;

using System;

public class CampaignCalculatorService
{
    public double GetPrice(string[] eanNumbers)
    {
        var products = this.ConvertToProducts(eanNumbers);
        var processedProducts = new List<Product>();

        while (products.Any())
        {
            var item = products[0];
            products.Remove(item);

            var comboItem = products.Find(x => x.ComboCategory == item.ComboCategory);

            if (comboItem == null)
            {
                processedProducts.Add(item with { CalculatedPrice = item.OriginalPrice });
            }
            else
            {
                processedProducts.Add(item with { CalculatedPrice = item.ComboPrice });
                processedProducts.Add(comboItem with { CalculatedPrice = comboItem.ComboPrice });
                products.Remove(comboItem);
            }
        }

        return processedProducts.Sum(x => x.CalculatedPrice);
    }

    private List<Product> ConvertToProducts(string[] eANNumbers)
    {
        return new List<Product>();
    }
}
