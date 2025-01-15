using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;

namespace PhysicalData.Application.Command.PhysicalDimension.Delete
{
    public sealed class DeletePhysicalDimensionCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
    {
        public required Guid PhysicalDimensionId { get; init; } = Guid.Empty;

        public required Guid RestrictedPassportId { get; init; }
    }
}
