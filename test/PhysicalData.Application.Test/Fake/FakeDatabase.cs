using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Test.Fake
{
    internal sealed class FakeDatabase
    {
        public IDictionary<Guid, PhysicalDimensionTransferObject> PhysicalDimension { get; } = new Dictionary<Guid, PhysicalDimensionTransferObject>();
        public IDictionary<Guid, TimePeriodTransferObject> TimePeriod { get; } = new Dictionary<Guid, TimePeriodTransferObject>();
    }
}
