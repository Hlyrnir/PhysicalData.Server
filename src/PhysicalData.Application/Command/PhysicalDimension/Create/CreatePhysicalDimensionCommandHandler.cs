using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.PhysicalDimension.Create
{
    internal sealed class CreatePhysicalDimensionCommandHandler : ICommandHandler<CreatePhysicalDimensionCommand, IMessageResult<Guid>>
    {
        private readonly TimeProvider prvTime;
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public CreatePhysicalDimensionCommandHandler(
            TimeProvider prvTime,
            IPhysicalDimensionRepository repoPhysicalDimension)
        {
            this.prvTime = prvTime;
            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<Guid>> Handle(CreatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

            Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Create(
                fExponentOfAmpere: msgMessage.ExponentOfAmpere,
                fExponentOfCandela: msgMessage.ExponentOfCandela,
                fExponentOfKelvin: msgMessage.ExponentOfKelvin,
                fExponentOfKilogram: msgMessage.ExponentOfKilogram,
                fExponentOfMetre: msgMessage.ExponentOfMetre,
                fExponentOfMole: msgMessage.ExponentOfMole,
                fExponentOfSecond: msgMessage.ExponentOfSecond,
                dConversionFactorToSI: msgMessage.ConversionFactorToSI,
                sCultureName: msgMessage.CultureName,
                sName: msgMessage.Name,
                sSymbol: msgMessage.Symbol,
                sUnit: msgMessage.Unit);

            if (pdPhysicalDimension is null)
                return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Physical dimension could not be created." });

            RepositoryResult<bool> bInsert = await repoPhysicalDimension.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), tknCancellation);

            return bInsert.Match(
                msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                bResult => new MessageResult<Guid>(pdPhysicalDimension.Id));
        }
    }
}