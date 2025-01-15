using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.PhysicalDimension.ById
{
    internal class PhysicalDimensionByIdValidation : IValidation<PhysicalDimensionByIdQuery>
    {
        private readonly TimeProvider prvTime;
        private readonly IMessageValidation srvValidation;

        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public PhysicalDimensionByIdValidation(IPhysicalDimensionRepository repoPhysicalDimension, IMessageValidation srvValidation, TimeProvider prvTime)
        {
            this.prvTime = prvTime;
            this.srvValidation = srvValidation;

            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<bool>> ValidateAsync(PhysicalDimensionByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            srvValidation.ValidateGuid(msgMessage.PhysicalDimensionId, "Physical dimension identifier");

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
            }

            return await Task.FromResult(
                srvValidation.Match(
                    msgError => new MessageResult<bool>(msgError),
                    bResult => new MessageResult<bool>(bResult))
                );
        }
    }
}
