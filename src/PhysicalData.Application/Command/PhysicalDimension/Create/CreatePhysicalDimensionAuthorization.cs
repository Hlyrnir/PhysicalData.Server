using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.PhysicalDimension.Create
{
    internal sealed class CreatePhysicalDimensionAuthorization : IAuthorization<CreatePhysicalDimensionCommand>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.PhysicalDimension;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Create;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(CreatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}