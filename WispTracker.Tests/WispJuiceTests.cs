using Xunit;

namespace WispTracker.Tests;

public class WispJuiceTests
{
    [Theory]
    [InlineData(45, 1000)]
    [InlineData(60, 2000)]
    [InlineData(90, 4000)]
    [InlineData(105, 5000)]
    public void PurpleExactCalibration(int areaPct, int juice) =>
        Assert.Equal(juice, WispJuice.GuessPurple(areaPct));

    [Fact]
    public void PurpleRoundedCalibration_701_is_41pct()
    {
        Assert.Equal(41, (int)Math.Round(WispJuice.PurpleBasePct + WispJuice.PurplePerThousand / 1000.0 * 701));
        Assert.Equal(733, WispJuice.GuessPurple(41));
    }

    [Fact]
    public void PurpleHighEndCalibration_8562_is_140pct()
    {
        Assert.Equal(140, WispJuice.ForwardPurple(8562));
        Assert.Equal(8500, WispJuice.GuessPurple(140));
    }

    [Theory]
    [InlineData(1000, 45)]
    [InlineData(2000, 60)]
    [InlineData(4000, 90)]
    [InlineData(5000, 105)]
    public void PurpleForwardLowEnd(int juice, int areaPct) =>
        Assert.Equal(areaPct, WispJuice.ForwardPurple(juice));

    [Theory]
    [InlineData(45, 2000)]
    [InlineData(65, 4000)]
    public void YellowExactCalibration(int velocityPct, int juice) =>
        Assert.Equal(juice, WispJuice.GuessYellow(velocityPct));

    [Fact]
    public void YellowTruncatedCalibration_3553_is_60pct()
    {
        Assert.Equal(60, WispJuice.YellowBasePct + WispJuice.YellowPerThousand * 3553 / 1000);
        Assert.Equal(3500, WispJuice.GuessYellow(60));
    }

    [Fact]
    public void YellowTruncatedCalibration_4307_is_68pct()
    {
        Assert.Equal(68, WispJuice.YellowBasePct + WispJuice.YellowPerThousand * 4307 / 1000);
        Assert.Equal(4300, WispJuice.GuessYellow(68));
    }

    [Fact]
    public void FormatIsJuiceThenStat() =>
        Assert.Equal("2000 (60%)", WispJuice.Format(2000, 60));

    [Theory]
    [InlineData(30)]
    [InlineData(0)]
    [InlineData(25)]
    public void PurpleAtOrBelowBaseIsZero(int areaPct) =>
        Assert.Equal(0, WispJuice.GuessPurple(areaPct));

    [Theory]
    [InlineData(25)]
    [InlineData(0)]
    [InlineData(10)]
    public void YellowAtOrBelowBaseIsZero(int velocityPct) =>
        Assert.Equal(0, WispJuice.GuessYellow(velocityPct));

    [Fact]
    public void HasteModStrips50VelocityBeforeGuess()
    {
        var adjusted = WispJuice.AdjustYellowVelocity(110, hasHasteMod: true);
        Assert.Equal(60, adjusted);
        Assert.Equal(3500, WispJuice.GuessYellow(adjusted));
    }

    [Fact]
    public void NoHasteModLeavesVelocityAlone()
    {
        Assert.Equal(65, WispJuice.AdjustYellowVelocity(65, hasHasteMod: false));
        Assert.Equal(4000, WispJuice.GuessYellow(WispJuice.AdjustYellowVelocity(65, false)));
    }

    [Fact]
    public void HasteModAloneDoesNotCountAsYellowJuice()
    {
        var adjusted = WispJuice.AdjustYellowVelocity(50, hasHasteMod: true);
        Assert.Equal(0, adjusted);
        Assert.Equal(0, WispJuice.GuessYellow(adjusted));
    }

    [Fact]
    public void MapMonsterMovementSpeedIsSubtracted()
    {
        var adjusted = WispJuice.AdjustYellowVelocity(75, hasHasteMod: false, mapMonstersMovementSpeedPct: 10);
        Assert.Equal(65, adjusted);
        Assert.Equal(4000, WispJuice.GuessYellow(adjusted));
    }

    [Fact]
    public void HasteAndMapMovementSpeedBothSubtract()
    {
        var adjusted = WispJuice.AdjustYellowVelocity(120, hasHasteMod: true, mapMonstersMovementSpeedPct: 10);
        Assert.Equal(60, adjusted);
        Assert.Equal(3500, WispJuice.GuessYellow(adjusted));
    }
}
