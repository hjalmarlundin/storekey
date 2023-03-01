namespace CampaignCalculatorApp;

public record Product
{
    public double OriginalPrice { get; init; }

    public string EAN { get; init; }

    public double ComboPrice { get; set; }

    public string ComboCategory { get; set; } = null!;

    public double VolumePrice { get; set; }

    public int VolumeQuantity { get; set; }

    public double CalculatedPrice { get; set; }

}
