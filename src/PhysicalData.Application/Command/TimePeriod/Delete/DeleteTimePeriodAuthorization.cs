using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.TimePeriod.Delete
{
    internal sealed class DeleteTimePeriodAuthorization : IAuthorization<DeleteTimePeriodCommand>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.TimePeriod;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Delete;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(DeleteTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}
