namespace Application.Work.Timing
{
    public sealed class TimingBarLayout
    {
        public required int TotalHeightPx { get; init; }

        public required int InitTotalPx { get; init; }

        public required int InitTrPx { get; init; }

        public int InitSpPx => InitTotalPx - InitTrPx;

        public required int InitTopY { get; init; }

        public required int WorkTotalPx { get; init; }

        public required int WorkWkPx { get; init; }

        public required int WorkTrPx { get; init; }

        public int WorkSpPx => WorkTotalPx - WorkWkPx - WorkTrPx;

        public required int WorkTopY { get; init; }

        public required int EndTotalPx { get; init; }

        public required int EndTrPx { get; init; }

        public int EndSpPx => EndTotalPx - EndTrPx;

        public required int EndTopY { get; init; }

        public int RegisterBracketTop => InitTopY;

        public int RegisterBracketBottom => EndTopY + EndTotalPx;

        public int WorkBracketTop => WorkTopY;

        public int WorkBracketBottom => WorkTopY + WorkTotalPx;
    }
}
