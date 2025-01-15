using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.PhysicalDimension.Delete
{
    internal sealed class DeletePhysicalDimensionCommandHandler : ICommandHandler<DeletePhysicalDimensionCommand, IMessageResult<bool>>
    {
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public DeletePhysicalDimensionCommandHandler(IPhysicalDimensionRepository repoPhysicalDimension)
        {
            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<bool>> Handle(DeletePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

            return await rsltPhysicalDimension.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async dtoPhysicalDimension =>
                {
                    RepositoryResult<bool> rsltDelete = await repoPhysicalDimension.DeleteAsync(dtoPhysicalDimension, tknCancellation);

                    return rsltDelete.Match(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<bool>(bResult));
                });
        }
    }
}