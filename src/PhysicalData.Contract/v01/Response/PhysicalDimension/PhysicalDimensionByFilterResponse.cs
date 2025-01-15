namespace PhysicalData.Contract.v01.Response.PhysicalDimension
{
    public sealed class PhysicalDimensionByFilterResponse : PagedResponse
    {
        public required IEnumerable<PhysicalDimensionByIdResponse> PhysicalDimension { get; init; }
    }
}