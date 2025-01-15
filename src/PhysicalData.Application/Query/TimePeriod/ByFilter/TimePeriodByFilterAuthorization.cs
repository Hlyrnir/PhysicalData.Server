using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.TimePeriod.ByFilter
{
    internal sealed class TimePeriodByFilterAuthorization : IAuthorization<TimePeriodByFilterQuery>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.TimePeriod;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Read;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(TimePeriodByFilterQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}