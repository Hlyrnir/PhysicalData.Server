using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.TimePeriod.ByFilter
{
    internal sealed class TimePeriodByFilterQueryHandler : IQueryHandler<TimePeriodByFilterQuery, IMessageResult<TimePeriodByFilterResult>>
    {
        private readonly ITimePeriodRepository repoTimePeriod;

        public TimePeriodByFilterQueryHandler(ITimePeriodRepository repoTimePeriod)
        {
            this.repoTimePeriod = repoTimePeriod;
        }

        public async ValueTask<IMessageResult<TimePeriodByFilterResult>> Handle(TimePeriodByFilterQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<TimePeriodByFilterResult>(DefaultMessageError.TaskAborted);

            RepositoryResult<int> rsltQuantity = await repoTimePeriod.QuantityByFilterAsync(msgMessage.Filter, tknCancellation);

            return await rsltQuantity.MatchAsync(
                msgError => new MessageResult<TimePeriodByFilterResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async iQuantity =>
                {
                    if (iQuantity < 1)
                    {
                        TimePeriodByFilterResult qryResult = new TimePeriodByFilterResult()
                        {
                            TimePeriod = Enumerable.Empty<TimePeriodTransferObject>(),
                            MaximalNumberOfTimePeriod = iQuantity
                        };

                        return new MessageResult<TimePeriodByFilterResult>(qryResult);
                    }

                    RepositoryResult<IEnumerable<TimePeriodTransferObject>> rsltTimePeriod = await repoTimePeriod.FindByFilterAsync(msgMessage.Filter, tknCancellation);

                    return rsltTimePeriod.Match(
                        msgError => new MessageResult<TimePeriodByFilterResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        enumTimePeriod =>
                        {
                            TimePeriodByFilterResult qryResult = new TimePeriodByFilterResult()
                            {
                                TimePeriod = enumTimePeriod,
                                MaximalNumberOfTimePeriod = iQuantity
                            };

                            return new MessageResult<TimePeriodByFilterResult>(qryResult);
                        });
                });
        }
    }
}
