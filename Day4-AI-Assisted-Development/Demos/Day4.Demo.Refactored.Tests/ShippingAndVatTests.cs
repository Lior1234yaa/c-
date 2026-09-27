using Day4.Demo.Refactored.Services;

namespace Day4.Demo.Refactored.Tests;

public class ShippingAndVatTests
{
    [Theory]
    [InlineData("IL", 199, 25)]
    [InlineData("IL", 200, 0)]
    [InlineData("US", 499, 60)]
    [InlineData("US", 500, 0)]
    [InlineData("DE", 100, 45)]
    [InlineData("FR", 400, 0)]
    [InlineData("JP", 10000, 80)]   // מדינה לא מוכרת — תעריף ברירת מחדל, בלי משלוח חינם
    public void Shipping_ByCountryAndThreshold(string country, decimal amount, decimal expected)
        => Assert.Equal(expected, new ShippingCalculator().Calculate(country, amount));

    [Fact]
    public void Shipping_CountryIsCaseInsensitive()
        => Assert.Equal(25m, new ShippingCalculator().Calculate("il", 10m));

    [Theory]
    [InlineData("IL", 100, 18)]
    [InlineData("US", 100, 0)]
    public void Vat_OnlyInIsrael(string country, decimal amount, decimal expected)
        => Assert.Equal(expected, new IsraelVatCalculator().Calculate(country, amount));
}
