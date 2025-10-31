using Domain.Work.Timing;

namespace Application.Work.Timing;

public sealed class TimingBarCalculator
{
    public static TimingBarLayout Calculate(TimingInfoHours t, int totalHeightPx, int minBandPx = 0)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentOutOfRangeException.ThrowIfNegative(totalHeightPx);
        t.EnsureValid();

        if (t.TimeTotalReg <= 0 || totalHeightPx == 0)
        {
            return new TimingBarLayout
            {
                TotalHeightPx = 0,
                InitTotalPx = 0,
                InitTrPx = 0,
                InitTopY = 0,
                WorkTotalPx = 0,
                WorkWkPx = 0,
                WorkTrPx = 0,
                WorkTopY = 0,
                EndTotalPx = 0,
                EndTrPx = 0,
                EndTopY = 0,
            };
        }

        var initTotalPx = Scale(totalHeightPx, t.TimeInit, t.TimeTotalReg);
        var workTotalPx = Scale(totalHeightPx, t.TimeWork, t.TimeTotalReg);
        var endTotalPx = totalHeightPx - initTotalPx - workTotalPx;

        var (initTrPx, initSpPx) = Split2(initTotalPx, t.TimeInitTR, t.TimeInitSP, minBandPx);
        var (workWkPx, workTrPx, workSpPx) = Split3(workTotalPx, t.TimeWorkWK, t.TimeWorkTR, t.TimeWorkSP, minBandPx);
        var (endTrPx, endSpPx) = Split2(endTotalPx, t.TimeEndTR, t.TimeEndSP, minBandPx);

        var initTopY = 0;
        var workTopY = initTopY + initTotalPx;
        var endTopY = workTopY + workTotalPx;

        if (initTrPx + initSpPx != initTotalPx) initSpPx = initTotalPx - initTrPx;
        if (workWkPx + workTrPx + workSpPx != workTotalPx) workSpPx = workTotalPx - workWkPx - workTrPx;
        if (endTrPx + endSpPx != endTotalPx) endSpPx = endTotalPx - endTrPx;

        return new TimingBarLayout
        {
            TotalHeightPx = totalHeightPx,
            InitTotalPx = initTotalPx,
            InitTrPx = initTrPx,
            InitTopY = initTopY,
            WorkTotalPx = workTotalPx,
            WorkWkPx = workWkPx,
            WorkTrPx = workTrPx,
            WorkTopY = workTopY,
            EndTotalPx = endTotalPx,
            EndTrPx = endTrPx,
            EndTopY = endTopY,
        };
    }

    private static int Scale(int totalPx, double partHours, double totalHours)
    {
        if (totalHours <= 0) return 0;
        var raw = totalPx * (partHours / totalHours);
        return (int)Math.Round(raw, MidpointRounding.AwayFromZero);
    }

    private static (int aPx, int bPx) Split2(int totalPx, double a, double b, int minBandPx)
    {
        var sum = a + b;
        if (sum <= 0) return (0, 0);

        var rawA = totalPx * (a / sum);
        var rawB = totalPx - rawA;

        var aPx = (int)Math.Floor(rawA);
        var bPx = totalPx - aPx;

        EnforceMin(ref aPx, ref bPx, totalPx, minBandPx);

        var deficit = totalPx - (aPx + bPx);
        if (deficit != 0)
        {
            if ((rawA - aPx) >= (rawB - (bPx - deficit)))
                aPx += deficit;
            else
                bPx += deficit;
        }

        return (aPx, bPx);
    }

    private static (int aPx, int bPx, int cPx) Split3(int totalPx, double a, double b, double c, int minBandPx)
    {
        var sum = a + b + c;
        if (sum <= 0) return (0, 0, 0);

        var rawA = totalPx * (a / sum);
        var rawB = totalPx * (b / sum);
        var rawC = totalPx - rawA - rawB;

        var aPx = (int)Math.Floor(rawA);
        var bPx = (int)Math.Floor(rawB);
        var cPx = totalPx - aPx - bPx;

        EnforceMin(ref aPx, ref bPx, ref cPx, totalPx, minBandPx);

        var deficit = totalPx - (aPx + bPx + cPx);
        if (deficit != 0)
        {
            var remainders = new[]
            {
                (idx: 0, rem: rawA - aPx),
                (idx: 1, rem: rawB - bPx),
                (idx: 2, rem: rawC - cPx),
            }
            .OrderByDescending(x => x.rem)
            .ToArray();

            var i = 0;
            while (deficit != 0)
            {
                switch (remainders[i % 3].idx)
                {
                    case 0: aPx += Math.Sign(deficit); break;
                    case 1: bPx += Math.Sign(deficit); break;
                    case 2: cPx += Math.Sign(deficit); break;
                }
                deficit -= Math.Sign(deficit);
                i++;
            }
        }

        return (aPx, bPx, cPx);
    }

    private static void EnforceMin(ref int aPx, ref int bPx, int totalPx, int min)
    {
        if (min <= 0) return;
        if (totalPx == 0) { aPx = 0; bPx = 0; return; }

        if (aPx < min) { bPx -= (min - aPx); aPx = min; }
        if (bPx < min) { aPx -= (min - bPx); bPx = min; }

        if (aPx < 0 || bPx < 0)
        {
            var half = totalPx / 2;
            aPx = Math.Max(0, half);
            bPx = totalPx - aPx;
        }
    }

    private static void EnforceMin(ref int aPx, ref int bPx, ref int cPx, int totalPx, int min)
    {
        if (min <= 0) return;
        if (totalPx == 0) { aPx = bPx = cPx = 0; return; }

        if (aPx < min) { var d = min - aPx; aPx += d; if (bPx >= cPx) bPx -= d; else cPx -= d; }
        if (bPx < min) { var d = min - bPx; bPx += d; if (aPx >= cPx) aPx -= d; else cPx -= d; }
        if (cPx < min) { var d = min - cPx; cPx += d; if (aPx >= bPx) aPx -= d; else bPx -= d; }

        if (aPx < 0 || bPx < 0 || cPx < 0)
        {
            aPx = Math.Max(0, totalPx / 3);
            bPx = Math.Max(0, (totalPx - aPx) / 2);
            cPx = totalPx - aPx - bPx;
        }
    }
}
