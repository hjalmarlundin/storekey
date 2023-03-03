namespace CampaignCalculatorApp;

using System;
using System.Linq;

public class VolumeCalculator : IVolumeCalculator
{
    public IEnumerable<Product> ProcessVolumeDiscounts(List<VolumeProduct> products)
    {
        var processedProducts = new List<Product>();

        // Volume discounts are calculated per EAN code
        foreach (var itemGroup in products.GroupBy(y => y.EAN))
        {
            var product = itemGroup.First();
            var numberOfItemsWithTheSameEANCode = itemGroup.Count();

            // There is not enough products to get any discount, keep original price as new price.
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

        return processedProducts;
    }
}
