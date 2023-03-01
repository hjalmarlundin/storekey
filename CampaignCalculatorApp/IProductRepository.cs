namespace CampaignCalculatorApp;

using System;

public interface IProductRepository
{
    IEnumerable<Product> Values { get; }
}
