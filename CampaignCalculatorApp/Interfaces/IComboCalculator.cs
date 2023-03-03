namespace CampaignCalculatorApp;

public interface IComboCalculator
{
    IEnumerable<Product> ProcessCombinationDiscounts(List<ComboProduct> products);
}
