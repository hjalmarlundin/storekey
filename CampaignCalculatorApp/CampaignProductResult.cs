namespace CampaignCalculatorApp;

public record CampaignProductResult
{
    public double CalculatedPrice { get; set; }

    public IEnumerable<Product> Products { get; set; } = null!;
}
