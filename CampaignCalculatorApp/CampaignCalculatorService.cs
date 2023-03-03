namespace CampaignCalculatorApp;

using System;
using System.Linq;

public class CampaignCalculatorService
{
    private readonly IProductRepository productRepository;

    public CampaignCalculatorService(IProductRepository products)
    {
        this.productRepository = products ?? throw new ArgumentNullException(nameof(products));
    }

    public CampaignProductResult CalculatePriceOnCheckout(string[] eanNumbers)
    {
        var products = this.ConvertToProducts(eanNumbers);
        var comboProducts = products.OfType<ComboProduct>().ToList();
        var volumeProducts = products.OfType<VolumeProduct>().ToList();
        var productsWithoutAnyDiscount = products.Except(comboProducts).Except(volumeProducts).ToList();

        if (products.Count != comboProducts.Count + volumeProducts.Count + productsWithoutAnyDiscount.Count)
        {
            throw new InvalidOperationException("Unexpected number of products after conversion to respective type");
        }

        var processedComboProducts = ProcessCombinationDiscounts(comboProducts);
        var processedVolumeProducts = ProcessVolumeDiscounts(volumeProducts);
        var processedOrdinaryProdcuts = ProcessResidualProducts(productsWithoutAnyDiscount);

        var totalNumberOfProducts = processedComboProducts.Concat(processedVolumeProducts).Concat(processedOrdinaryProdcuts);

        if (totalNumberOfProducts.Count() != products.Count)
        {
            throw new InvalidOperationException("Expected the same number of products processed as the input");
        }

        return new CampaignProductResult() { CalculatedPrice = totalNumberOfProducts.Sum(x => x.CalculatedPrice), Products = totalNumberOfProducts };
    }

    private static IEnumerable<Product> ProcessResidualProducts(IEnumerable<Product> products)
    {
        var processedProducts = new List<Product>();

        // These items does not have neither volume or combo discount.
        foreach (var item in products)
        {
            processedProducts.Add(item with { CalculatedPrice = item.OriginalPrice });
        }

        return processedProducts;
    }

    private static IEnumerable<Product> ProcessVolumeDiscounts(List<VolumeProduct> products)
    {
        var processedProducts = new List<Product>();

        // Volume discounts are calculated per EAN code
        foreach (var itemGroup in products.Where(x => x.VolumeQuantity > 0).GroupBy(y => y.EAN))
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

    private static IEnumerable<Product> ProcessCombinationDiscounts(List<ComboProduct> products)
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

    private List<Product> ConvertToProducts(string[] eanNumbers)
    {
        var products = new List<Product>();
        foreach (var eanNumber in eanNumbers)
        {
            var item = this.productRepository.Values.SingleOrDefault(x => x.EAN == eanNumber);
            if (item == null)
            {
                throw new ArgumentException($"Could not find any product in database with code: {eanNumber}");
            }

            products.Add(item);
        }

        if (products.Count != eanNumbers.Length)
        {
            throw new InvalidOperationException("Unexpected number of products after conversion from EAN number");
        }

        return products;
    }
}
