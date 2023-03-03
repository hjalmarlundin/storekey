namespace CampaignCalculatorApp.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using CampaignCalculatorApp;
using FluentAssertions;
using Xunit;

public class VolumeCalculatorTests
{
    [Fact]
    public void Volume_correct_quantity_should_get_discount()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<VolumeProduct>
        {
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 },
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 }
        };

        // Act
        var result = sut.ProcessVolumeDiscounts(products);

        // Assert
        result.Should().AllSatisfy(x => x.CalculatedPrice = 30);
    }

    [Fact]
    public void Volume_an_additional_item_does_not_give_discount()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<VolumeProduct>
        {
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 },
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 },
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 }
        };

        // Act
        var result = sut.ProcessVolumeDiscounts(products);

        // Assert
        result.Where(x => x.CalculatedPrice == 30).Should().HaveCount(2);
        result.Where(x => x.CalculatedPrice == 50).Should().HaveCount(1);
    }

    [Fact]
    public void Volume_two_sets_both_gets_discount()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<VolumeProduct>
        {
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 },
            new VolumeProduct() { EAN = "Item1", OriginalPrice = 50, VolumeQuantity = 2, VolumePrice = 30 },
            new VolumeProduct() { EAN = "Item2", OriginalPrice = 60, VolumeQuantity = 2, VolumePrice = 40 },
            new VolumeProduct() { EAN = "Item2", OriginalPrice = 60, VolumeQuantity = 2, VolumePrice = 40 },
        };

        // Act
        var result = sut.ProcessVolumeDiscounts(products);

        // Assert
        result.Where(x => x.CalculatedPrice == 30).Should().HaveCount(2);
        result.Where(x => x.CalculatedPrice == 40).Should().HaveCount(2);
    }

    private static VolumeCalculator CreateSut() => new VolumeCalculator();
}
