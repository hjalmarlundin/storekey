namespace CampaignCalculatorApp;

public record Product
{
    public double OriginalPrice { get; set; }

    public string EAN { get; set; }

    public double ComboPrice { get; set; }

    public string ComboCategory { get; set; }

    public double VolumePrice { get; set; }

    public int VolumeQuantity { get; set; }

    public double CalculatedPrice { get; set; }

}
