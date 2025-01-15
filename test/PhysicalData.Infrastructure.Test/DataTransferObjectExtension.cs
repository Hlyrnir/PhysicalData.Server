using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test
{
    internal static class DataTransferObjectExtension
    {
        internal static PhysicalDimensionTransferObject Clone(this PhysicalDimensionTransferObject dtoPhysicalDimension, bool bResetConcurrencyStamp = false)
        {
            string sConcurrencyStamp = dtoPhysicalDimension.ConcurrencyStamp;

            if (bResetConcurrencyStamp == true)
                sConcurrencyStamp = Guid.NewGuid().ToString();

            return new PhysicalDimensionTransferObject()
            {
                ConcurrencyStamp = sConcurrencyStamp,
                ConversionFactorToSI = dtoPhysicalDimension.ConversionFactorToSI,
                CultureName = dtoPhysicalDimension.CultureName,
                ExponentOfAmpere = dtoPhysicalDimension.ExponentOfAmpere,
                ExponentOfCandela = dtoPhysicalDimension.ExponentOfCandela,
                ExponentOfKelvin = dtoPhysicalDimension.ExponentOfKelvin,
                ExponentOfKilogram = dtoPhysicalDimension.ExponentOfKilogram,
                ExponentOfMetre = dtoPhysicalDimension.ExponentOfMetre,
                ExponentOfMole = dtoPhysicalDimension.ExponentOfMole,
                ExponentOfSecond = dtoPhysicalDimension.ExponentOfSecond,
                Id = dtoPhysicalDimension.Id,
                Name = dtoPhysicalDimension.Name,
                Symbol = dtoPhysicalDimension.Symbol,
                Unit = dtoPhysicalDimension.Unit
            };
        }

        internal static TimePeriodTransferObject Clone(this TimePeriodTransferObject dtoTimePeriod, bool bResetConcurrencyStamp = false)
        {
            string sConcurrencyStamp = dtoTimePeriod.ConcurrencyStamp;

            if (bResetConcurrencyStamp == true)
                sConcurrencyStamp = Guid.NewGuid().ToString();

            return new TimePeriodTransferObject()
            {
                ConcurrencyStamp = sConcurrencyStamp,
                Id = dtoTimePeriod.Id,
                Magnitude = dtoTimePeriod.Magnitude,
                Offset = dtoTimePeriod.Offset,
                PhysicalDimensionId = dtoTimePeriod.PhysicalDimensionId
            };
        }
    }
}