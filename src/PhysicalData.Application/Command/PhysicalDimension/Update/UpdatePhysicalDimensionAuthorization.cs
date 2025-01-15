using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Command.PhysicalDimension.Update
{
    internal sealed class UpdatePhysicalDimensionAuthorization : IAuthorization<UpdatePhysicalDimensionCommand>
    {
        public string PassportVisaName { get; } = DefaultPassportVisa.Name.PhysicalDimension;
        public int PassportVisaLevel { get; } = DefaultPassportVisa.Level.Update;

        public async ValueTask<IMessageResult<bool>> AuthorizeAsync(UpdatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            return new MessageResult<bool>(true);
        }
    }
}