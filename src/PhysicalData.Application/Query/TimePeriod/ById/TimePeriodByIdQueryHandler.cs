using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.TimePeriod.ById
{
    internal sealed class TimePeriodByIdQueryHandler : IQueryHandler<TimePeriodByIdQuery, IMessageResult<TimePeriodByIdResult>>
    {
        private readonly ITimePeriodRepository repoTimePeriod;

        public TimePeriodByIdQueryHandler(ITimePeriodRepository repoTimePeriod)
        {
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<TimePeriodByIdResult>> Handle(TimePeriodByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<TimePeriodByIdResult>(DefaultMessageError.TaskAborted);

            RepositoryResult<TimePeriodTransferObject> rsltTimePeriod = await repoTimePeriod.FindByIdAsync(msgMessage.TimePeriodId, tknCancellation);

            return rsltTimePeriod.Match(
                msgError => new MessageResult<TimePeriodByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                pdTimePeriod =>
                {
                    TimePeriodByIdResult qryResult = new TimePeriodByIdResult()
                    {
                        TimePeriod = pdTimePeriod
                    };

                    return new MessageResult<TimePeriodByIdResult>(qryResult);
                });
        }
    }
}