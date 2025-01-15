using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.PhysicalDimension.ById
{
    internal sealed class PhysicalDimensionByIdAuthorization : IAuthorization<PhysicalDimensionByIdQuery>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.PhysicalDimension;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Read;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(PhysicalDimensionByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}
