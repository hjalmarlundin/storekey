namespace CampaignCalculatorApp;

public record VolumeProduct : Product
{
    public double VolumePrice { get; set; }

    public int VolumeQuantity { get; set; }
}
