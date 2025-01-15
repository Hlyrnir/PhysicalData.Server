using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Extension
{
    internal static class TimePeriodExtension
    {
        internal static Domain.Aggregate.TimePeriod? Initialize(this TimePeriodTransferObject dtoTimePeriod)
        {
            return Domain.Aggregate.TimePeriod.Initialize(
                sConcurrencyStamp: dtoTimePeriod.ConcurrencyStamp,
                guId: dtoTimePeriod.Id,
                dMagnitude: dtoTimePeriod.Magnitude,
                dOffset: dtoTimePeriod.Offset,
                guPhysicalDimensionId: dtoTimePeriod.PhysicalDimensionId);
        }

        internal static TimePeriodTransferObject MapToTransferObject(this Domain.Aggregate.TimePeriod pdTimePeriod)
        {
            return new TimePeriodTransferObject()
            {
                ConcurrencyStamp = pdTimePeriod.ConcurrencyStamp,
                Id = pdTimePeriod.Id,
                Magnitude = pdTimePeriod.Magnitude,
                Offset = pdTimePeriod.Offset,
                PhysicalDimensionId = pdTimePeriod.PhysicalDimensionId
            };
        }
    }
}