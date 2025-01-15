using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.PhysicalDimension.Delete
{
    internal class DeletePhysicalDimensionValidation : IValidation<DeletePhysicalDimensionCommand>
    {
        private readonly IMessageValidation srvValidation;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public DeletePhysicalDimensionValidation(IMessageValidation srvValidation, IPhysicalDimensionRepository repoPhysicalDimension)
        {
            this.srvValidation = srvValidation;
            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<bool>> ValidateAsync(DeletePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            RepositoryResult<bool> rsltPhysicalDimension = await repoPhysicalDimension.ExistsAsync(msgMessage.PhysicalDimensionId, tknCancellation);

            rsltPhysicalDimension.Match(
                msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult =>
                {
                    if (bResult == false)
                        srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Physical dimension {msgMessage.PhysicalDimensionId} does not exist." });

                    return bResult;
                });

            return srvValidation.Match(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<bool>(bResult));
        }
    }
}