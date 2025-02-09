namespace PhysicalData.DataFaker
{
    public static class PhysicalDimension
    {
        public static class ElectricCurrent
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 1,
                    fExponentOfCandela: 0,
                    fExponentOfKelvin: 0,
                    fExponentOfKilogram: 0,
                    fExponentOfMetre: 0,
                    fExponentOfMole: 0,
                    fExponentOfSecond: 0,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Electric current",
                    sSymbol: "I",
                    sUnit: "A");

                if (pdPhysicalDimension is null)
                    throw new NullReferenceException();

                return pdPhysicalDimension;
            }
        }

        public static class LuminousIntensity
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 0,
                    fExponentOfCandela: 1,
                    fExponentOfKelvin: 0,
                    fExponentOfKilogram: 0,
                    fExponentOfMetre: 0,
                    fExponentOfMole: 0,
                    fExponentOfSecond: 0,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Luminous intensity",
                    sSymbol: "J",
                    sUnit: "cd");

                if (pdPhysicalDimension is null)
                    throw new NullReferenceException();

                return pdPhysicalDimension;
            }
        }

        public static class ThermodynamicTemperature
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 0,
                    fExponentOfCandela: 0,
                    fExponentOfKelvin: 1,
                    fExponentOfKilogram: 0,
                    fExponentOfMetre: 0,
                    fExponentOfMole: 0,
                    fExponentOfSecond: 0,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Thermodynamic temperature",
                    sSymbol: "T",
                    sUnit: "K");

                if (pdPhysicalDimension is null)
                    throw new NullReferenceException();

                return pdPhysicalDimension;
            }
        }

        public static class Mass
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 0,
                    fExponentOfCandela: 0,
                    fExponentOfKelvin: 0,
                    fExponentOfKilogram: 1,
                    fExponentOfMetre: 0,
                    fExponentOfMole: 0,
                    fExponentOfSecond: 0,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Mass",
                    sSymbol: "m",
                    sUnit: "kg");

                if (pdPhysicalDimension is null)
                    throw new NullReferenceException();

                return pdPhysicalDimension;
            }
        }

        public static class Length
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? ppPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 0,
                    fExponentOfCandela: 0,
                    fExponentOfKelvin: 0,
                    fExponentOfKilogram: 0,
                    fExponentOfMetre: 1,
                    fExponentOfMole: 0,
                    fExponentOfSecond: 0,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Metre",
                    sSymbol: "l",
                    sUnit: "m");

                if (ppPhysicalDimension is null)
                    throw new NullReferenceException();

                return ppPhysicalDimension;
            }
        }

        public static class AmountOfSubstance
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 0,
                    fExponentOfCandela: 0,
                    fExponentOfKelvin: 0,
                    fExponentOfKilogram: 0,
                    fExponentOfMetre: 0,
                    fExponentOfMole: 1,
                    fExponentOfSecond: 0,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Amount of substance",
                    sSymbol: "N",
                    sUnit: "mol");

                if (pdPhysicalDimension is null)
                    throw new NullReferenceException();

                return pdPhysicalDimension;
            }
        }

        public static class Time
        {
            public static Domain.Aggregate.PhysicalDimension CreateDefault()
            {
                Domain.Aggregate.PhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalDimension.Initialize(
                    fExponentOfAmpere: 0,
                    fExponentOfCandela: 0,
                    fExponentOfKelvin: 0,
                    fExponentOfKilogram: 0,
                    fExponentOfMetre: 0,
                    fExponentOfMole: 0,
                    fExponentOfSecond: 1,
                    sConcurrencyStamp: Guid.NewGuid().ToString(),
                    dConversionFactorToSI: 1,
                    sCultureName: "en-GB",
                    guId: Guid.NewGuid(),
                    sName: "Time",
                    sSymbol: "t",
                    sUnit: "s");

                if (pdPhysicalDimension is null)
                    throw new NullReferenceException();

                return pdPhysicalDimension;
            }
        }
    }
}