using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.TimePeriod.Update
{
    internal sealed class UpdateTimePeriodCommandHandler : ICommandHandler<UpdateTimePeriodCommand, IMessageResult<bool>>
    {
        private readonly TimeProvider prvTime;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;
        private readonly ITimePeriodRepository repoTimePeriod;

        public UpdateTimePeriodCommandHandler(
            TimeProvider prvTime,
            IPhysicalDimensionRepository repoPhysicalDimension,
            ITimePeriodRepository repoTimePeriod)
        {
            this.prvTime = prvTime;
            this.repoPhysicalDimension = repoPhysicalDimension;
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<bool>> Handle(UpdateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

            return await rsltPhysicalDimension.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async dtoPhysicalDimension =>
                {
                    Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = dtoPhysicalDimension.Initialize();

                    if (pdPhysicalDimension is null)
                        return new MessageResult<bool>(DomainError.InitializationHasFailed);

                    RepositoryResult<TimePeriodTransferObject> rsltTimePeriod = await repoTimePeriod.FindByIdAsync(msgMessage.TimePeriodId, tknCancellation);

                    return await rsltTimePeriod.MatchAsync(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        async dtoTimePeriod =>
                        {
                            Domain.Aggregate.TimePeriod? pdTimePeriod = dtoTimePeriod.Initialize();

                            if (pdTimePeriod is null)
                                return new MessageResult<bool>(DomainError.InitializationHasFailed);

                            if (pdTimePeriod.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
                                return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

                            if (pdTimePeriod.TryChangePhysicalDimension(pdPhysicalDimension) == false)
                                return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Physical dimension could not be changed." });

                            pdTimePeriod.Magnitude = msgMessage.Magnitude;
                            pdTimePeriod.Offset = msgMessage.Offset;

                            RepositoryResult<bool> rsltUpdate = await repoTimePeriod.UpdateAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), tknCancellation);

                            return rsltUpdate.Match(
                                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                                bResult => new MessageResult<bool>(bResult));
                        });
                });
        }
    }
}