using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.TimePeriod.ById
{
    internal class TimePeriodByIdValidation : IValidation<TimePeriodByIdQuery>
    {
        private readonly TimeProvider prvTime;
        private readonly IMessageValidation srvValidation;

        private readonly ITimePeriodRepository repoTimePeriod;

        public TimePeriodByIdValidation(ITimePeriodRepository repoTimePeriod, IMessageValidation srvValidation, TimeProvider prvTime)
        {
            this.prvTime = prvTime;
            this.srvValidation = srvValidation;

            this.repoTimePeriod = repoTimePeriod;
        }

        async ValueTask<IMessageResult<bool>> IValidation<TimePeriodByIdQuery>.ValidateAsync(TimePeriodByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            srvValidation.ValidateGuid(msgMessage.TimePeriodId, "Time period identifier");

            if (srvValidation.IsValid == true)
            {
                RepositoryResult<bool> rsltTimePeriod = await repoTimePeriod.ExistsAsync(msgMessage.TimePeriodId, tknCancellation);

                rsltTimePeriod.Match(
                    msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                    bResult =>
                    {
                        if (bResult == false)
                            srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Time period {msgMessage.TimePeriodId} does not exist." });

                        return bResult;
                    });
            }

            return await Task.FromResult(
                srvValidation.Match(
                    msgError => new MessageResult<bool>(msgError),
                    bResult => new MessageResult<bool>(bResult))
                );
        }
    }
}
