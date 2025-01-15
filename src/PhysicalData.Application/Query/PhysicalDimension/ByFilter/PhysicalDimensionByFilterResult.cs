using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.PhysicalDimension.ByFilter
{
    public sealed class PhysicalDimensionByFilterResult
    {
        public required IEnumerable<PhysicalDimensionTransferObject> PhysicalDimension { get; init; }
        public required int MaximalNumberOfPhysicalDimension { get; init; }
    }
}
