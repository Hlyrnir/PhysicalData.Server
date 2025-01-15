namespace PhysicalData.Contract.v01.Response.TimePeriod
{
    public class TimePeriodByFilterResponse : PagedResponse
    {
        public required IEnumerable<TimePeriodByIdResponse> TimePeriod { get; init; }
    }
}
