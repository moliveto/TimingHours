using Application.Work.Timing;
using Domain.Work.Timing;
using Xunit;

public sealed class TimingBarCalculatorTests
{
    [Fact]
    public void Zero_Total_Produces_Zero_Layout()
    {
        var t = TimingInfoHours.CreateAuto(0, 0, 0, 0, 0, 0, 0);
        var l = TimingBarCalculator.Calculate(t, totalHeightPx: 200);
        Assert.Equal(0, l.TotalHeightPx);
        Assert.Equal(0, l.InitTotalPx + l.WorkTotalPx + l.EndTotalPx);
    }

    [Fact]
    public void Sum_Blocks_Equals_Total_Height()
    {
        var t = TimingInfoHours.CreateAuto(1, 0.2, 5, 3.5, 0.8, 0.5, 0.1);
        var l = TimingBarCalculator.Calculate(t, totalHeightPx: 200);
        Assert.Equal(200, l.InitTotalPx + l.WorkTotalPx + l.EndTotalPx);
    }

    [Fact]
    public void Work_SubBands_Sum_Equals_Work_Block()
    {
        var t = TimingInfoHours.CreateAuto(1, 0.2, 5, 3.5, 0.8, 0.5, 0.1);
        var l = TimingBarCalculator.Calculate(t, 300);
        Assert.Equal(l.WorkTotalPx, l.WorkWkPx + l.WorkTrPx + l.WorkSpPx);
    }

    [Fact]
    public void Order_Offsets_TopY_Are_Stacked()
    {
        var t = TimingInfoHours.CreateAuto(1, 0.2, 5, 3.5, 0.8, 0.5, 0.1);
        var l = TimingBarCalculator.Calculate(t, 240);
        Assert.Equal(0, l.InitTopY);
        Assert.Equal(l.InitTotalPx, l.WorkTopY);
        Assert.Equal(l.InitTotalPx + l.WorkTotalPx, l.EndTopY);
    }

    [Fact]
    public void MinBandPx_Enforced_On_Tiny_Segments()
    {
        var t = TimingInfoHours.CreateAuto(
            timeInit: 0.3, timeInitTR: 0.01,
            timeWork: 3.0, timeWorkWK: 2.9, timeWorkTR: 0.09,
            timeEnd: 0.1, timeEndTR: 0.005);

        var l = TimingBarCalculator.Calculate(t, 120, minBandPx: 2);
        Assert.True(l.InitTrPx >= 2 || l.InitTrPx == 0);
        Assert.True(l.WorkTrPx >= 2 || l.WorkTrPx == 0);
    }

    [Fact]
    public void LargestRemainder_Distributes_Rounding_Fairly()
    {
        var t = new TimingInfoHours
        {
            TimeInit = 3, TimeInitTR = 1, TimeInitSP = 2,
            TimeWork = 3, TimeWorkWK = 1, TimeWorkTR = 1, TimeWorkSP = 1,
            TimeEnd = 3, TimeEndTR = 1, TimeEndSP = 2,
            TimeTotalReg = 9
        };

        var l = TimingBarCalculator.Calculate(t, 101);
        Assert.InRange(l.InitTotalPx, 33, 35);
        Assert.InRange(l.WorkTotalPx, 33, 35);
        Assert.InRange(l.EndTotalPx, 33, 35);
        Assert.Equal(101, l.InitTotalPx + l.WorkTotalPx + l.EndTotalPx);
    }
}
