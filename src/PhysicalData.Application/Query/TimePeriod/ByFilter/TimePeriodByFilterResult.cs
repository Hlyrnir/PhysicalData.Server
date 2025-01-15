using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.TimePeriod.ByFilter
{
    public sealed class TimePeriodByFilterResult
    {
        public required IEnumerable<TimePeriodTransferObject> TimePeriod { get; init; }
        public required int MaximalNumberOfTimePeriod { get; init; }
    }
}
