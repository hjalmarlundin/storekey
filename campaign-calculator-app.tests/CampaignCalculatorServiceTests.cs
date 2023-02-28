namespace CampaignCalculatorApp.Tests;

using FluentAssertions;
using Xunit;
using System;

public class CampaignCalculatorServiceTests
{
    [Fact]
    public void UnKnown_EAN_Should_Throw()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        Assert.Throws<ArgumentException>(() => sut.GetPrice(new[] { "UnknownEAN", "Combo1" }));
    }

    [Fact]
    public void Combo_two_identical_should_get_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo1" });

        // Assert
        result.Should().Be(30.0);
    }

    [Fact]
    public void Combo_two_unique_should_get_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo2" });

        // Assert
        result.Should().Be(30.0);
    }

    [Fact]
    public void Combo_does_not_give_discount_if_only_one_exist()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "NonCombo1" });

        // Assert
        result.Should().Be(40.0);
    }

    [Fact]
    public void Combo_two_sets_gets_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo2", "Combo3", "Combo4" });

        // Assert
        result.Should().Be(70.0);
    }

    [Fact]
    public void Combo_two_sets_with_only_two_discounts_gets_one_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "NonCombo1", "NonCombo2", "Combo2" });

        // Assert
        result.Should().Be(80.0);
    }

    [Fact]
    public void Combo_three_only_gives_discount_for_two()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo2", "Combo3" });

        // Assert
        result.Should().Be(60.0);
    }

    [Fact]
    public void Combo_with_three_combos_and_two_non_combos_should_get_one_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo2", "Combo3", "NonCombo1", "NonCombo2" });

        // Assert
        result.Should().Be(110.0);
    }

    [Fact]
    public void Volume_correct_quantity_should_get_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Volume1", "Volume1" });

        // Assert
        result.Should().Be(40.0);
    }

    [Fact]
    public void Volume_an_additional_item_does_not_give_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Volume1", "Volume1", "Volume1" });

        // Assert
        result.Should().Be(70.0);
    }

    [Fact]
    public void Volume_two_different_items_both_gets_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Volume1", "Volume1", "Volume2", "Volume2", "Volume2", "NonVolume1" });

        // Assert
        result.Should().Be(155.0);
    }

    private static CampaignCalculatorService CreateSut() => new CampaignCalculatorService(new Products());
}
