using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Extension
{
    internal static class PhysicalDimensionExtension
    {
        internal static Domain.Aggregate.PhysicalDimension? Initialize(this PhysicalDimensionTransferObject dtoPhysicalDimension)
        {
            return Domain.Aggregate.PhysicalDimension.Initialize(
                sConcurrencyStamp: dtoPhysicalDimension.ConcurrencyStamp,
                dConversionFactorToSI: dtoPhysicalDimension.ConversionFactorToSI,
                sCultureName: dtoPhysicalDimension.CultureName,
                fExponentOfAmpere: dtoPhysicalDimension.ExponentOfAmpere,
                fExponentOfCandela: dtoPhysicalDimension.ExponentOfCandela,
                fExponentOfKelvin: dtoPhysicalDimension.ExponentOfKelvin,
                fExponentOfKilogram: dtoPhysicalDimension.ExponentOfKilogram,
                fExponentOfMetre: dtoPhysicalDimension.ExponentOfMetre,
                fExponentOfMole: dtoPhysicalDimension.ExponentOfMole,
                fExponentOfSecond: dtoPhysicalDimension.ExponentOfSecond,
                guId: dtoPhysicalDimension.Id,
                sName: dtoPhysicalDimension.Name,
                sSymbol: dtoPhysicalDimension.Symbol,
                sUnit: dtoPhysicalDimension.Unit);
        }

        internal static PhysicalDimensionTransferObject MapToTransferObject(this Domain.Aggregate.PhysicalDimension pdPhysicalDimension)
        {
            return new PhysicalDimensionTransferObject()
            {
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = pdPhysicalDimension.ConversionFactorToSI,
                CultureName = pdPhysicalDimension.CultureName,
                ExponentOfAmpere = pdPhysicalDimension.ExponentOfUnit.Ampere,
                ExponentOfCandela = pdPhysicalDimension.ExponentOfUnit.Candela,
                ExponentOfKelvin = pdPhysicalDimension.ExponentOfUnit.Kelvin,
                ExponentOfKilogram = pdPhysicalDimension.ExponentOfUnit.Kilogram,
                ExponentOfMetre = pdPhysicalDimension.ExponentOfUnit.Metre,
                ExponentOfMole = pdPhysicalDimension.ExponentOfUnit.Mole,
                ExponentOfSecond = pdPhysicalDimension.ExponentOfUnit.Second,
                Id = pdPhysicalDimension.Id,
                Name = pdPhysicalDimension.Name,
                Symbol = pdPhysicalDimension.Symbol,
                Unit = pdPhysicalDimension.Unit
            };
        }
    }
}
