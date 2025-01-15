using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;

namespace PhysicalData.Application.Command.TimePeriod.Create
{
    public sealed class CreateTimePeriodCommand : ICommand<IMessageResult<Guid>>, IRestrictedAuthorization
    {
        public required Guid RestrictedPassportId { get; init; }

        public required Guid PhysicalDimensionId { get; init; } = Guid.Empty;
        public required double[] Magnitude { get; init; } = new double[] { 0.0 };
        public required double Offset { get; init; } = 0.0;
    }
}
