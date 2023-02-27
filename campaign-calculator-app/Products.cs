namespace CampaignCalculatorApp;

public class Products
{
    public Products()
    {
        this.Values = new List<Product>
        {
            new Product() { EAN = "Combo1", OriginalPrice = 20, ComboCategory = "Food", ComboPrice = 15 },
            new Product() { EAN = "Combo2", OriginalPrice = 20, ComboCategory = "Food", ComboPrice = 15 },
            new Product() { EAN = "Combo3", OriginalPrice = 30, ComboCategory = "Clothes", ComboPrice = 20 },
            new Product() { EAN = "Combo4", OriginalPrice = 20, ComboCategory = "Clothes", ComboPrice = 20 },
            new Product() { EAN = "NonCombo1", OriginalPrice = 20 },
            new Product() { EAN = "NonCombo2", OriginalPrice = 30 }
        };
    }

    public IEnumerable<Product> Values { get; }
}
