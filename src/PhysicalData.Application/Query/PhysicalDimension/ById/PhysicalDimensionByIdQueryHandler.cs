using Mediator;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Query.PhysicalDimension.ById
{
    internal sealed class PhysicalDimensionByIdQueryHandler : IQueryHandler<PhysicalDimensionByIdQuery, IMessageResult<PhysicalDimensionByIdResult>>
    {
        private readonly IPhysicalDimensionRepository repoPhysicalDimension;

        public PhysicalDimensionByIdQueryHandler(IPhysicalDimensionRepository repoPhysicalDimension)
        {
            this.repoPhysicalDimension = repoPhysicalDimension;
        }

        public async ValueTask<IMessageResult<PhysicalDimensionByIdResult>> Handle(PhysicalDimensionByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<PhysicalDimensionByIdResult>(DefaultMessageError.TaskAborted);

            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

            return rsltPhysicalDimension.Match(
                msgError => new MessageResult<PhysicalDimensionByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                pdPhysicalDimension =>
                {
                    PhysicalDimensionByIdResult qryResult = new PhysicalDimensionByIdResult()
                    {
                        PhysicalDimension = pdPhysicalDimension
                    };

                    return new MessageResult<PhysicalDimensionByIdResult>(qryResult);
                });
        }
    }
}
