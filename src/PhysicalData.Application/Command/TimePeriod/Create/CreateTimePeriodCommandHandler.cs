using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.TimePeriod.Create
{
    internal sealed class CreateTimePeriodCommandHandler : ICommandHandler<CreateTimePeriodCommand, IMessageResult<Guid>>
    {
        private readonly TimeProvider prvTime;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;
        private readonly ITimePeriodRepository repoTimePeriod;

        public CreateTimePeriodCommandHandler(
            TimeProvider prvTime,
            IPhysicalDimensionRepository repoPhysicalDimension,
            ITimePeriodRepository repoTimePeriod)
        {
            this.prvTime = prvTime;
            this.repoPhysicalDimension = repoPhysicalDimension;
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<Guid>> Handle(CreateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

            return await rsltPhysicalDimension.MatchAsync(
                msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async dtoPhysicalDimension =>
                {
                    Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = dtoPhysicalDimension.Initialize();

                    if (pdPhysicalDimension is null)
                        return new MessageResult<Guid>(DomainError.InitializationHasFailed);

                    Domain.Aggregate.TimePeriod? pdTimePeriod = Domain.Aggregate.TimePeriod.Create(
                        dMagnitude: msgMessage.Magnitude,
                        dOffset: msgMessage.Offset,
                        pdPhysicalDimension: pdPhysicalDimension);

                    if (pdTimePeriod is null)
                        return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Time period could not be created." });

                    RepositoryResult<bool> rsltInsert = await repoTimePeriod.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), tknCancellation);

                    return rsltInsert.Match(
                        msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<Guid>(pdTimePeriod.Id));
                });
        }
    }
}