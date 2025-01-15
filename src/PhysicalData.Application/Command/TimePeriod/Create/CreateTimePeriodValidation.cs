using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.TimePeriod.Create
{
    internal class CreateTimePeriodValidation : IValidation<CreateTimePeriodCommand>
    {
        private readonly IMessageValidation srvValidation;
        private readonly ITimePeriodRepository repoTimePeriod;

        public CreateTimePeriodValidation(IMessageValidation srvValidation, ITimePeriodRepository repoTimePeriod)
        {
            this.srvValidation = srvValidation;
            this.repoTimePeriod = repoTimePeriod;
        }

        async ValueTask<IMessageResult<bool>> IValidation<CreateTimePeriodCommand>.ValidateAsync(CreateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            TimePeriodByFilterOption optFilter = new TimePeriodByFilterOption()
            {
                PhysicalDimensionId = msgMessage.PhysicalDimensionId,
                Magnitude = msgMessage.Magnitude,
                Offset = msgMessage.Offset,
                Page = 1,
                PageSize = 1
            };

            RepositoryResult<IEnumerable<TimePeriodTransferObject>> rsltPhysicalDimension = await repoTimePeriod.FindByFilterAsync(optFilter, tknCancellation);

            rsltPhysicalDimension.Match(
                msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                enumPhysicalDimension =>
                {
                    if (enumPhysicalDimension.Any() == true)
                        srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Time period does exist." });

                    return true;
                });

            return srvValidation.Match(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<bool>(bResult));
        }
    }
}