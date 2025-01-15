using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.TimePeriod.Create
{
    internal sealed class CreateTimePeriodAuthorization : IAuthorization<CreateTimePeriodCommand>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.TimePeriod;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Create;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(CreateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}
