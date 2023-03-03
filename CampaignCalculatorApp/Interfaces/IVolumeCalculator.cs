namespace CampaignCalculatorApp;

public interface IVolumeCalculator
{
    IEnumerable<Product> ProcessVolumeDiscounts(List<VolumeProduct> products);
}
