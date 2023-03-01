namespace CampaignCalculatorApp;

using System;
using System.Linq;

public class CampaignCalculatorService
{
    private readonly IProductRepository products;

    public CampaignCalculatorService(IProductRepository products)
    {
        this.products = products ?? throw new ArgumentNullException(nameof(products));
    }
    public double GetPrice(string[] eanNumbers)
    {
        var products = this.ConvertToProducts(eanNumbers);
        var numberOfProducts = products.Count;
        var processedProducts = new List<Product>();

        ProcessCombinationDiscounts(products, processedProducts);

        products = ProcessVolumeDiscounts(products, processedProducts);

        ProcessResidualProducts(products, processedProducts);

        if (processedProducts.Count != numberOfProducts)
        {
            throw new Exception("Oh no");
        }

        return processedProducts.Sum(x => x.CalculatedPrice);
    }

    private static void ProcessResidualProducts(List<Product> products, List<Product> processedProducts)
    {
        foreach (var item in products)
        {
            processedProducts.Add(item with { CalculatedPrice = item.OriginalPrice });
        }
    }

    private static List<Product> ProcessVolumeDiscounts(List<Product> products, List<Product> processedProducts)
    {
        foreach (var itemGroup in products.Where(x => x.VolumeQuantity > 0).GroupBy(y => y.EAN))
        {

            var product = itemGroup.First();
            var numberOfItemsWithTheSameEANCode = itemGroup.Count();
            products = products.Except(itemGroup).ToList();

            if (product.VolumeQuantity > numberOfItemsWithTheSameEANCode)
            {
                for (int i = 0; i < numberOfItemsWithTheSameEANCode; i++)
                {
                    processedProducts.Add(product with { CalculatedPrice = product.OriginalPrice });
                }
            }
            else
            {
                var numberOfItemsWithoutDiscount = numberOfItemsWithTheSameEANCode % product.VolumeQuantity;
                var numberOfItemsWithDiscount = numberOfItemsWithTheSameEANCode - numberOfItemsWithoutDiscount;
                for (int i = 0; i < numberOfItemsWithoutDiscount; i++)
                {
                    processedProducts.Add(product with { CalculatedPrice = product.OriginalPrice });
                }
                for (int i = 0; i < numberOfItemsWithDiscount; i++)
                {
                    processedProducts.Add(product with { CalculatedPrice = product.VolumePrice });
                }
            }
        }

        return products;
    }

    private static void ProcessCombinationDiscounts(List<Product> products, List<Product> processedProducts)
    {
        while (products.Any(x => x.ComboCategory != null))
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
