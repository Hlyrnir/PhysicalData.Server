using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.TimePeriod.Delete
{
    internal class DeleteTimePeriodValidation : IValidation<DeleteTimePeriodCommand>
    {
        private readonly IMessageValidation srvValidation;
        private readonly ITimePeriodRepository repoTimePeriod;

        public DeleteTimePeriodValidation(
            IMessageValidation srvValidation,
            ITimePeriodRepository repoTimePeriod)
        {
            this.srvValidation = srvValidation;
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<bool>> ValidateAsync(DeleteTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            RepositoryResult<bool> rsltTimePeriod = await repoTimePeriod.ExistsAsync(msgMessage.TimePeriodId, tknCancellation);

            rsltTimePeriod.Match(
                msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult =>
                {
                    if (bResult == false)
                        srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Time period {msgMessage.TimePeriodId} does not exist." });

                    return bResult;
                });

            return srvValidation.Match(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<bool>(bResult));
        }
    }
}