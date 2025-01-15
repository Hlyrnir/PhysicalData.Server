namespace PhysicalData.Contract.v01.Response
{
    public abstract class PagedResponse
    {
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public required int ResultCount { get; init; }
    }
}
