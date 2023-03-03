namespace CampaignCalculatorApp;

using System;
using System.Linq;

public class CampaignCalculatorService
{
    private readonly IProductRepository productRepository;
    private readonly IComboCalculator comboCalculator;
    private readonly IVolumeCalculator volumeCalculator;

    public CampaignCalculatorService(IProductRepository products, IComboCalculator comboCalculator, IVolumeCalculator volumeCalculator)
    {
        this.volumeCalculator = volumeCalculator ?? throw new ArgumentNullException(nameof(volumeCalculator));
        this.comboCalculator = comboCalculator ?? throw new ArgumentNullException(nameof(comboCalculator));
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

        var processedComboProducts = this.comboCalculator.ProcessCombinationDiscounts(comboProducts);
        var processedVolumeProducts = this.volumeCalculator.ProcessVolumeDiscounts(volumeProducts);
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
