using BifServiceExpenditureMaterials.Helpers;

namespace BifServiceExpenditureMaterials.Tests;

/// <summary>
/// Юнит-тесты для вспомогательного класса <see cref="Other"/>.
/// </summary>
public class OtherHelperTests
{
    // ─── GetMouthNumber(string) ────────────────────────────────────────────────

    [Theory]
    [InlineData("Январь",   1)]
    [InlineData("Февраль",  2)]
    [InlineData("Март",     3)]
    [InlineData("Апрель",   4)]
    [InlineData("Май",      5)]
    [InlineData("Июнь",     6)]
    [InlineData("Июль",     7)]
    [InlineData("Август",   8)]
    [InlineData("Сентябрь", 9)]
    [InlineData("Октябрь",  10)]
    [InlineData("Ноябрь",   11)]
    [InlineData("Декабрь",  12)]
    public void GetMouthNumber_KnownMonthName_ReturnsCorrectNumber(string name, int expected)
    {
        var result = Other.GetMouthNumber(name);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetMouthNumber_UnknownMonthName_Returns1()
    {
        var result = Other.GetMouthNumber("Неизвестный");
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetMouthNumber_EmptyString_Returns1()
    {
        var result = Other.GetMouthNumber("");
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetMouthNumber_NullString_Returns1()
    {
        var result = Other.GetMouthNumber(null!);
        Assert.Equal(1, result);
    }

    // ─── GetMouthNumber(int) ───────────────────────────────────────────────────

    [Theory]
    [InlineData(1,  "Январь")]
    [InlineData(2,  "Февраль")]
    [InlineData(3,  "Март")]
    [InlineData(4,  "Апрель")]
    [InlineData(5,  "Май")]
    [InlineData(6,  "Июнь")]
    [InlineData(7,  "Июль")]
    [InlineData(8,  "Август")]
    [InlineData(9,  "Сентябрь")]
    [InlineData(10, "Октябрь")]
    [InlineData(11, "Ноябрь")]
    [InlineData(12, "Декабрь")]
    public void GetMouthNumber_ValidMonthNumber_ReturnsCorrectName(int number, string expected)
    {
        var result = Other.GetMouthNumber(number);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetMouthNumber_ZeroNumber_ReturnsFallback()
    {
        var result = Other.GetMouthNumber(0);
        Assert.Equal("Январь", result);
    }

    [Fact]
    public void GetMouthNumber_NegativeNumber_ReturnsFallback()
    {
        var result = Other.GetMouthNumber(-1);
        Assert.Equal("Январь", result);
    }

    [Fact]
    public void GetMouthNumber_NumberOver12_ReturnsFallback()
    {
        var result = Other.GetMouthNumber(13);
        Assert.Equal("Январь", result);
    }

    // ─── Симметрия ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void GetMouthNumber_Roundtrip_IsSymmetric(int monthNum)
    {
        var name = Other.GetMouthNumber(monthNum);       // int → string
        var back = Other.GetMouthNumber(name);           // string → int
        Assert.Equal(monthNum, back);
    }

    // ─── GetAllMonths ──────────────────────────────────────────────────────────

    [Fact]
    public void GetAllMonths_Returns13Items_FirstIsEmpty()
    {
        var months = Other.GetAllMonths();
        Assert.Equal(13, months.Count);
        Assert.Equal("", months[0]);
    }

    [Fact]
    public void GetAllMonths_ContainsAllMonthNames()
    {
        var months = Other.GetAllMonths();
        var expected = new[]
        {
            "Январь","Февраль","Март","Апрель","Май","Июнь",
            "Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь"
        };
        foreach (var m in expected)
            Assert.Contains(m, months);
    }

    // ─── GetDayList ────────────────────────────────────────────────────────────

    [Fact]
    public void GetDayList_Returns31Items()
    {
        var days = Other.GetDayList();
        Assert.Equal(31, days.Count);
    }

    [Fact]
    public void GetDayList_FirstIs1_LastIs31()
    {
        var days = Other.GetDayList();
        Assert.Equal("1", days[0]);
        Assert.Equal("31", days[30]);
    }

    // ─── GetYearList ───────────────────────────────────────────────────────────

    [Fact]
    public void GetYearList_ContainsCurrent2025And2026()
    {
        var years = Other.GetYearList();
        Assert.Contains("2025", years);
        Assert.Contains("2026", years);
    }

    [Fact]
    public void GetYearList_NoEmptyEntries()
    {
        var years = Other.GetYearList();
        Assert.All(years, y => Assert.False(string.IsNullOrEmpty(y)));
    }

    // ─── GetValueInYacheyka ────────────────────────────────────────────────────

    [Theory]
    [InlineData("МАШ-001:15", 15)]
    [InlineData("prefix:0",    0)]
    [InlineData("abc:31",      31)]
    [InlineData(":7",          7)]
    public void GetValueInYacheyka_ValidInput_ReturnsCorrectValue(string input, int expected)
    {
        var result = Other.GetValueInYacheyka(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetValueInYacheyka_EmptyString_Returns0()
    {
        Assert.Equal(0, Other.GetValueInYacheyka(""));
    }

    [Fact]
    public void GetValueInYacheyka_Null_Returns0()
    {
        Assert.Equal(0, Other.GetValueInYacheyka(null!));
    }

    [Fact]
    public void GetValueInYacheyka_NoColon_Returns0()
    {
        Assert.Equal(0, Other.GetValueInYacheyka("нетдвоеточия"));
    }

    [Fact]
    public void GetValueInYacheyka_NonNumericAfterColon_Returns0()
    {
        Assert.Equal(0, Other.GetValueInYacheyka("prefix:abc"));
    }
}
