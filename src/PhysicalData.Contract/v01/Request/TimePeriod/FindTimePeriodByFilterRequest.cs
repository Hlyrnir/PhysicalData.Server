namespace PhysicalData.Contract.v01.Request.TimePeriod
{
    public sealed class FindTimePeriodByFilterRequest : PagedRequest
    {
        public required Guid? PhysicalDimensionId { get; init; }
        public required string? Magnitude { get; init; }
        public required double? Offset { get; init; }
    }
}
