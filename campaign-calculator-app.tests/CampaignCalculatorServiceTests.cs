using Xunit;
using FluentAssertions;

namespace CampaignCalculatorApp.Tests;

public class CampaignCalculatorServiceTests
{
    [Fact]
    public void UnKnown_EAN_Should_Throw()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "UnknownEAN" });

        // Assert
        result.Should().Be(5.0);
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
        result.Should().Be(60.0);
    }

    [Fact]
    public void Combo_two_sets_with_only_two_discounts_gets_one_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "NonCombo1", "NonCombo2", "Combo2" });

        // Assert
        result.Should().Be(70.0);
    }

    [Fact]
    public void Combo_three_only_gives_discount_for_two()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo2", "Combo3" });

        // Assert
        result.Should().Be(50.0);
    }

    [Fact]
    public void Combo_with_three_combos_and_two_non_combos_should_get_one_discount()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetPrice(new[] { "Combo1", "Combo2", "Combo3", "NonCombo1", "NonCombo2" });

        // Assert
        result.Should().Be(90.0);
    }

    private static CampaignCalculatorService CreateSut() => new CampaignCalculatorService();
}
