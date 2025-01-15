using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.PhysicalDimension.Update
{
    internal sealed class UpdatePhysicalDimensionCommandHandler : ICommandHandler<UpdatePhysicalDimensionCommand, IMessageResult<bool>>
    {
        private readonly TimeProvider prvTime;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public UpdatePhysicalDimensionCommandHandler(
            TimeProvider prvTime,
            IPhysicalDimensionRepository repoPhysicalDimension)
        {
            this.prvTime = prvTime;
            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<bool>> Handle(UpdatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

            return await rsltPhysicalDimension.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async dtoPhysicalDimension =>
                {
                    PhysicalData.Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = dtoPhysicalDimension.Initialize();

                    if (pdPhysicalDimension is null)
                        return new MessageResult<bool>(DomainError.InitializationHasFailed);

                    if (pdPhysicalDimension.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
                        return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

                    if (pdPhysicalDimension.TryChangeCultureName(msgMessage.CultureName) == false)
                        return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Culture name is not valid." });

                    pdPhysicalDimension.ConversionFactorToSI = msgMessage.ConversionFactorToSI;
                    pdPhysicalDimension.Name = msgMessage.Name;
                    pdPhysicalDimension.Symbol = msgMessage.Symbol;
                    pdPhysicalDimension.Unit = msgMessage.Unit;

                    RepositoryResult<bool> rsltUpdate = await repoPhysicalDimension.UpdateAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), tknCancellation);

                    return rsltUpdate.Match(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<bool>(bResult));
                });
        }
    }
}