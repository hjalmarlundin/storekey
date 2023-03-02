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

    public CampaignProductResult GetPrice(string[] eanNumbers)
    {
        var products = this.ConvertToProducts(eanNumbers);
        var numberOfProducts = products.Count;
        var processedProducts = new List<Product>();

        ProcessCombinationDiscounts(products, processedProducts);

        ProcessVolumeDiscounts(products, processedProducts);

        ProcessResidualProducts(products, processedProducts);

        if (processedProducts.Count != numberOfProducts)
        {
            throw new Exception("Expected the same number of products returned as received");
        }

        var campaignProductResult = new CampaignProductResult() { CalculatedPrice = processedProducts.Sum(x => x.CalculatedPrice), Products = processedProducts };
        return campaignProductResult;
    }

    private static void ProcessResidualProducts(List<Product> products, List<Product> processedProducts)
    {
        // These items does not have neither volume or combo discount.
        foreach (var item in products)
        {
            processedProducts.Add(item with { CalculatedPrice = item.OriginalPrice });
        }
    }

    private static void ProcessVolumeDiscounts(List<Product> products, List<Product> processedProducts)
    {
        // Volume discounts are calculated per EAN code
        foreach (var itemGroup in products.Where(x => x.VolumeQuantity > 0).GroupBy(y => y.EAN))
        {
            var product = itemGroup.First();
            var numberOfItemsWithTheSameEANCode = itemGroup.Count();
            products.RemoveAll(x => x.EAN == product.EAN);

            // There is not enough products to get any discount
            if (product.VolumeQuantity > numberOfItemsWithTheSameEANCode)
            {
                for (int i = 0; i < numberOfItemsWithTheSameEANCode; i++)
                {
                    processedProducts.Add(product with { CalculatedPrice = product.OriginalPrice });
                }
            }
            else
            {
                // There is enough products to get discount, check how many of them should get it.
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
    }

    private static void ProcessCombinationDiscounts(List<Product> products, List<Product> processedProducts)
    {
        // Process all items with combo category until empty
        while (products.Any(x => x.ComboCategory != null))
        {
            var item = products.First(x => x.ComboCategory != null);
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
