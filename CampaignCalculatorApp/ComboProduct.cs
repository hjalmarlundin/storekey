namespace CampaignCalculatorApp;

public record ComboProduct : Product
{
    public double ComboPrice { get; set; }

    public string ComboCategory { get; set; } = default!;
}
