namespace PhysicalData.DataFaker
{
    public static class TimePeriod
    {
        private static readonly double[] dMagnitude = { 0.0, 1.0, 2.0, 3.0, 4.0};

        public static Domain.Aggregate.TimePeriod CreateDefault(Guid guPhysicalDimensionId)
        {
            Domain.Aggregate.TimePeriod? pdTimePeriod = Domain.Aggregate.TimePeriod.Initialize(
                sConcurrencyStamp: Guid.NewGuid().ToString(),
                guId: Guid.NewGuid(),
                dMagnitude: dMagnitude,
                dOffset: 0.0,
                guPhysicalDimensionId: guPhysicalDimensionId);

            if (pdTimePeriod is null)
                throw new NullReferenceException();

            return pdTimePeriod;
        }
    }
}