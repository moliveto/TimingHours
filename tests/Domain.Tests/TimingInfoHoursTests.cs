using Domain.Work.Timing;
using Xunit;

public sealed class TimingInfoHoursTests
{
    [Fact]
    public void CreateAuto_Derives_SP_And_Total()
    {
        var t = TimingInfoHours.CreateAuto(
            timeInit: 1.0, timeInitTR: 0.2,
            timeWork: 5.0, timeWorkWK: 3.5, timeWorkTR: 0.8,
            timeEnd: 0.5, timeEndTR: 0.1);

        Assert.Equal(0.8, t.TimeInitSP, 3);
        Assert.Equal(0.7, t.TimeWorkSP, 3);
        Assert.Equal(0.4, t.TimeEndSP, 3);
        Assert.Equal(6.5, t.TimeTotalReg, 3);
    }

    [Fact]
    public void EnsureValid_Ok_When_Sums_Close()
    {
        var t = new TimingInfoHours
        {
            TimeInit = 1,
            TimeInitTR = 0.3,
            TimeInitSP = 0.7,

            TimeWork = 2,
            TimeWorkWK = 0.9,
            TimeWorkTR = 0.6,
            TimeWorkSP = 0.5,

            TimeEnd = 1,
            TimeEndTR = 0.2,
            TimeEndSP = 0.8,

            TimeTotalReg = 4,
        };

        t.EnsureValid();
    }

    [Fact]
    public void EnsureValid_Throws_When_Sums_DontMatch()
    {
        var t = new TimingInfoHours
        {
            TimeInit = 1, TimeInitTR = 0.5, TimeInitSP = 0.6,
            TimeWork = 2, TimeWorkWK = 0.5, TimeWorkTR = 0.5, TimeWorkSP = 1.0,
            TimeEnd = 1, TimeEndTR = 0.2, TimeEndSP = 0.8,
            TimeTotalReg = 4
        };

        Assert.Throws<TimingValidationException>(() => t.EnsureValid());
    }

    [Fact]
    public void EnsureValid_Throws_On_Negative()
    {
        var t = TimingInfoHours.CreateAuto(1, -0.1, 2, 1, 0.5, 1, 0.2);
        var ex = Assert.Throws<TimingValidationException>(() => t.EnsureValid());
        Assert.Contains("TimeInitTR <0.", ex.Message);
    }
}
