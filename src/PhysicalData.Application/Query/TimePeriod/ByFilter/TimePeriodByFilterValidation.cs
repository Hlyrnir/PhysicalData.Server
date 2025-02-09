using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.TimePeriod.ByFilter
{
    internal class TimePeriodByFilterValidation : IValidation<TimePeriodByFilterQuery>
    {
        private readonly IMessageValidation srvValidation;

        public TimePeriodByFilterValidation(IMessageValidation srvValidation)
        {
            this.srvValidation = srvValidation;
        }

        async ValueTask<IMessageResult<bool>> IValidation<TimePeriodByFilterQuery>.ValidateAsync(TimePeriodByFilterQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            if (msgMessage.Filter.Page <= 0)
                srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Page has to be greater than zero." });

            if (msgMessage.Filter.PageSize <= 0)
                srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Page size has to be greater than zero." });

            if (msgMessage.Filter.Magnitude is not null)
                srvValidation.ValidateAgainstSqlInjection(msgMessage.Filter.Magnitude, "Magnitude");

            return await Task.FromResult(srvValidation.Match(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<bool>(bResult)));
        }
    }
}
