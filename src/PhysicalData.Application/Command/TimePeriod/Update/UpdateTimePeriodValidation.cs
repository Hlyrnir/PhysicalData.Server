using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.TimePeriod.Update
{
    internal class UpdateTimePeriodValidation : IValidation<UpdateTimePeriodCommand>
    {
        private readonly IMessageValidation srvValidation;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;
        private readonly ITimePeriodRepository repoTimePeriod;

        public UpdateTimePeriodValidation(
            IMessageValidation srvValidation,
            IPhysicalDimensionRepository repoPhysicalDimension,
            ITimePeriodRepository repoTimePeriod)
        {
            this.srvValidation = srvValidation;
            this.repoPhysicalDimension = repoPhysicalDimension;
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<bool>> ValidateAsync(UpdateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            srvValidation.ValidateGuid(msgMessage.PhysicalDimensionId, "Physical dimension identifer");
            srvValidation.ValidateGuid(msgMessage.TimePeriodId, "Time period identifier");

            if (srvValidation.IsValid == true)
            {
                RepositoryResult<bool> rsltPhysicalDimension = await repoPhysicalDimension.ExistsAsync(msgMessage.PhysicalDimensionId, tknCancellation);

                rsltPhysicalDimension.Match(
                    msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                    bResult =>
                    {
                        if (bResult == false)
                            srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Physical dimension {msgMessage.PhysicalDimensionId} does not exist." });

                        return bResult;
                    });

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

            return srvValidation.Match(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<bool>(bResult));
        }
    }
}
