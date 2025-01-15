using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.TimePeriod.ById
{
    public sealed class TimePeriodByIdResult
    {
        public required TimePeriodTransferObject TimePeriod { get; init; }
    }
}
