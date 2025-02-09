namespace PhysicalData.Application.Filter
{
    public class TimePeriodByFilterOption
    {
        public required Guid? PhysicalDimensionId { get; init; }
        public required string? Magnitude { get; init; }
        public required double? Offset { get; init; }

        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
