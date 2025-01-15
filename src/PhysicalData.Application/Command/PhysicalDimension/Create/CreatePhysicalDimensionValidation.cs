using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Command.PhysicalDimension.Create
{
    internal class CreatePhysicalDimensionValidation : IValidation<CreatePhysicalDimensionCommand>
    {
        private readonly IMessageValidation srvValidation;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public CreatePhysicalDimensionValidation(IMessageValidation srvValidation, IPhysicalDimensionRepository repoPhysicalDimension)
        {
            this.srvValidation = srvValidation;
            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<bool>> ValidateAsync(CreatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            srvValidation.ValidateAgainstSqlInjection(msgMessage.CultureName, "Culture name");
            srvValidation.ValidateAgainstSqlInjection(msgMessage.Name, "Name");
            srvValidation.ValidateAgainstSqlInjection(msgMessage.Symbol, "Symbol");
            srvValidation.ValidateAgainstSqlInjection(msgMessage.Unit, "Unit");

            PhysicalDimensionByFilterOption optFilter = new PhysicalDimensionByFilterOption()
            {
                ConversionFactorToSI = msgMessage.ConversionFactorToSI,
                CultureName = msgMessage.CultureName,
                ExponentOfAmpere = msgMessage.ExponentOfAmpere,
                ExponentOfCandela = msgMessage.ExponentOfCandela,
                ExponentOfKelvin = msgMessage.ExponentOfKelvin,
                ExponentOfKilogram = msgMessage.ExponentOfKilogram,
                ExponentOfMetre = msgMessage.ExponentOfMetre,
                ExponentOfMole = msgMessage.ExponentOfMole,
                ExponentOfSecond = msgMessage.ExponentOfSecond,
                Name = msgMessage.Name,
                Symbol = msgMessage.Symbol,
                Unit = msgMessage.Unit,
                Page = 1,
                PageSize = 1
            };

            RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>> rsltPhysicalDimension = await repoPhysicalDimension.FindByFilterAsync(optFilter, tknCancellation);

            rsltPhysicalDimension.Match(
                msgError =>
                {
                    if(msgError.Code!= PhysicalDimensionError.Code.Method && msgError.Description!= "No data has been found.")
                        srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description });

                    return false;
                },
                enumPhysicalDimension =>
                {
                    if (enumPhysicalDimension.Any() == true)
                        srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Physical dimension does exist." });

                    return true;
                });

            return srvValidation.Match(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<bool>(bResult));
        }
    }
}
