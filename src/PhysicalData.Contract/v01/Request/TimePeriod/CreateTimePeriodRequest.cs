﻿namespace PhysicalData.Contract.v01.Request.TimePeriod
{
    public sealed class CreateTimePeriodRequest
    {
        public required Guid PhysicalDimensionId { get; init; }
        public required double[] Magnitude { get; init; }
        public required double Offset { get; init; }
    }
}
