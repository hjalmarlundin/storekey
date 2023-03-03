namespace CampaignCalculatorApp.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using CampaignCalculatorApp;
using FluentAssertions;
using Xunit;

public class ComboCalculatorTests
{
    [Fact]
    public void Combo_two_identical_should_get_discount()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<ComboProduct>();
        var item = new ComboProduct() { EAN = "Combo1", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 };
        products.Add(item);
        products.Add(item);

        // Act
        var result = sut.ProcessCombinationDiscounts(products);

        // Assert
        var expectedResult = item with { CalculatedPrice = 30 };
        result.Should().AllBeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Combo_two_diffrent_EAN_in_same_category_should_get_discount()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<ComboProduct>
        {
            new ComboProduct() { EAN = "Combo1", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
            new ComboProduct() { EAN = "Combo2", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 }
        };

        // Act
        var result = sut.ProcessCombinationDiscounts(products);

        // Assert
        result.Should().AllSatisfy(x => x.CalculatedPrice = 30);
    }

    [Fact]
    public void Combo_does_not_give_discount_if_only_one_exist()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<ComboProduct>
        {
            new ComboProduct() { EAN = "Combo1", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
        };

        // Act
        var result = sut.ProcessCombinationDiscounts(products);

        // Assert
        result.Should().AllSatisfy(x => x.CalculatedPrice = 50);
    }

    [Fact]
    public void Combo_two_sets_gets_discount()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<ComboProduct>
        {
            new ComboProduct() { EAN = "Combo1", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
            new ComboProduct() { EAN = "Combo2", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
            new ComboProduct() { EAN = "Combo3", OriginalPrice = 10, ComboCategory = "B", ComboPrice = 5 },
            new ComboProduct() { EAN = "Combo4", OriginalPrice = 10, ComboCategory = "B", ComboPrice = 5 }
        };

        // Act
        var result = sut.ProcessCombinationDiscounts(products);

        // Assert
        result.Where(x => x.CalculatedPrice == 30).Should().HaveCount(2);
        result.Where(x => x.CalculatedPrice == 5).Should().HaveCount(2);
    }

    [Fact]
    public void Combo_three_only_gives_discount_for_two()
    {
        // Arrange
        var sut = CreateSut();
        var products = new List<ComboProduct>
        {
            new ComboProduct() { EAN = "Combo1", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
            new ComboProduct() { EAN = "Combo2", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
            new ComboProduct() { EAN = "Combo3", OriginalPrice = 50, ComboCategory = "A", ComboPrice = 30 },
        };

        // Act
        var result = sut.ProcessCombinationDiscounts(products);

        // Assert
        result.Where(x => x.CalculatedPrice == 30).Should().HaveCount(2);
        result.Where(x => x.CalculatedPrice == 50).Should().HaveCount(1);
    }

    private static ComboCalculator CreateSut() => new ComboCalculator();
}
