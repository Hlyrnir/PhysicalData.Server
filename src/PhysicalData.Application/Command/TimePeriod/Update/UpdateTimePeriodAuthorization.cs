using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.TimePeriod.Update
{
    internal sealed class UpdateTimePeriodAuthorization : IAuthorization<UpdateTimePeriodCommand>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.TimePeriod;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Update;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(UpdateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}
