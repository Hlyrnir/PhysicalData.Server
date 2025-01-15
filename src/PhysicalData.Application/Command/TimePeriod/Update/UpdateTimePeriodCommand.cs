﻿using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;

namespace PhysicalData.Application.Command.TimePeriod.Update
{
    public sealed class UpdateTimePeriodCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
    {
        public required Guid RestrictedPassportId { get; init; }
        public required string ConcurrencyStamp { get; init; }

        public required double[] Magnitude { get; init; } = new double[] { 0.0 };
        public required double Offset { get; init; } = 0.0;
        public required Guid PhysicalDimensionId { get; init; }
        public required Guid TimePeriodId { get; init; }
    }
}
