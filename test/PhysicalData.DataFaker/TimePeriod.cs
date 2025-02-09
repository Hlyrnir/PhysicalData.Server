namespace PhysicalData.DataFaker
{
    public static class TimePeriod
    {
        public static Domain.Aggregate.TimePeriod CreateDefault(Guid guPhysicalDimensionId)
        {
            Random rndmGenerator = new Random();

            int iSize = 5;
            double[] dMagnitude = new double[iSize];

            for (int i = 0; i < iSize; i++)
            {
                dMagnitude[i] = rndmGenerator.NextDouble();
            }

            double dOffset = rndmGenerator.NextDouble();

            Domain.Aggregate.TimePeriod? pdTimePeriod = Domain.Aggregate.TimePeriod.Initialize(
                sConcurrencyStamp: Guid.NewGuid().ToString(),
                guId: Guid.NewGuid(),
                dMagnitude: dMagnitude,
                dOffset: dOffset,
                guPhysicalDimensionId: guPhysicalDimensionId);

            if (pdTimePeriod is null)
                throw new NullReferenceException();

            return pdTimePeriod;
        }
    }
}