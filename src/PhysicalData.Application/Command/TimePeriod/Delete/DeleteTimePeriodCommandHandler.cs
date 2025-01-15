using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.TimePeriod.Delete
{
    internal sealed class DeleteTimePeriodCommandHandler : ICommandHandler<DeleteTimePeriodCommand, IMessageResult<bool>>
    {
        private readonly ITimePeriodRepository repoTimePeriod;

        public DeleteTimePeriodCommandHandler(ITimePeriodRepository repoTimePeriod)
        {
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<bool>> Handle(DeleteTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            RepositoryResult<TimePeriodTransferObject> rsltTimePeriod = await repoTimePeriod.FindByIdAsync(msgMessage.TimePeriodId, tknCancellation);

            return await rsltTimePeriod.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async dtoTimePeriod =>
                {
                    RepositoryResult<bool> rsltDelete = await repoTimePeriod.DeleteAsync(dtoTimePeriod, tknCancellation);

                    return rsltDelete.Match(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<bool>(bResult));
                });
        }
    }
}