namespace CampaignCalculatorApp;

public record Product
{
    public double OriginalPrice { get; init; }

    public string EAN { get; init; } = default!;

    public double CalculatedPrice { get; set; }
}
