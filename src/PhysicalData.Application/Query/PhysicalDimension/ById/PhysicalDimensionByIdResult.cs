using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.PhysicalDimension.ById
{
    public sealed class PhysicalDimensionByIdResult
    {
        public required PhysicalDimensionTransferObject PhysicalDimension { get; init; }
    }
}
