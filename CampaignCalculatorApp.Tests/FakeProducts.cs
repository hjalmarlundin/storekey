namespace CampaignCalculatorApp.Tests;

using System.Collections.Generic;
using CampaignCalculatorApp;

public class FakeProducts : IProductRepository
{
    public FakeProducts()
    {
        this.Values = new List<Product>
        {
            new ComboProduct() { EAN = "Combo1", OriginalPrice = 20, ComboCategory = "Food", ComboPrice = 15 },
            new ComboProduct() { EAN = "Combo2", OriginalPrice = 20, ComboCategory = "Food", ComboPrice = 15 },
            new ComboProduct() { EAN = "Combo3", OriginalPrice = 30, ComboCategory = "Clothes", ComboPrice = 20 },
            new ComboProduct() { EAN = "Combo4", OriginalPrice = 20, ComboCategory = "Clothes", ComboPrice = 20 },
            new Product() { EAN = "NonCombo1", OriginalPrice = 20 },
            new Product() { EAN = "NonCombo2", OriginalPrice = 30 },
            new VolumeProduct() { EAN = "Volume1", OriginalPrice = 30, VolumePrice = 20, VolumeQuantity = 2 },
            new VolumeProduct() { EAN = "Volume2", OriginalPrice = 40, VolumePrice = 30, VolumeQuantity = 3 },
            new Product() { EAN = "StandardItem1", OriginalPrice = 25 },
        };
    }

    public IEnumerable<Product> Values { get; }
}
