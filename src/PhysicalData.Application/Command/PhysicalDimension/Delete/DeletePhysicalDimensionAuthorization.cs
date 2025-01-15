using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.PhysicalDimension.Delete
{
    internal sealed class DeletePhysicalDimensionAuthorization : IAuthorization<DeletePhysicalDimensionCommand>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.PhysicalDimension;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Delete;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(DeletePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}