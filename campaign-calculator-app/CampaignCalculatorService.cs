namespace CampaignCalculatorApp;

using System;
using System.Linq;

public class CampaignCalculatorService
{
    private readonly Products products;

    public CampaignCalculatorService(Products products)
    {
        this.products = products ?? throw new ArgumentNullException(nameof(products));
    }
    public double GetPrice(string[] eanNumbers)
    {
        var products = this.ConvertToProducts(eanNumbers);
        var processedProducts = new List<Product>();

        while (products.Any())
        {
            var item = products[0];
            products.Remove(item);

            var comboItem = products.Find(x => x.ComboCategory == item.ComboCategory);

            if (comboItem == null || item.ComboCategory == null)
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

    private List<Product> ConvertToProducts(string[] eanNumbers)
    {
        var products = new List<Product>();
        foreach (var eanNumber in eanNumbers)
        {
            if (!this.products.Values.Any(x => x.EAN == eanNumber))
            {
                throw new ArgumentException($"Could not find any product in database with code: {eanNumber}");
            }
            products.Add(this.products.Values.Single(x => x.EAN == eanNumber));
        }
        return products;
    }
}
