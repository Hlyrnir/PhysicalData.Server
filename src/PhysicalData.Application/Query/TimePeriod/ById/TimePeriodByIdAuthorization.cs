using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.TimePeriod.ById
{
    internal sealed class TimePeriodByIdAuthorization : IAuthorization<TimePeriodByIdQuery>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.TimePeriod;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Read;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(TimePeriodByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}