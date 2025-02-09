using FluentAssertions;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class PhysicalDimensionRepositorySpecification_FindByFilterAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public PhysicalDimensionRepositorySpecification_FindByFilterAsync(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task FindByFilter_ShouldReturnPhysicalDimension_WhenFilterIsNull()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_ElectricCurrent = DataFaker.PhysicalDimension.ElectricCurrent.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_LuminousIntensity = DataFaker.PhysicalDimension.LuminousIntensity.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_ThermodynamicTemperature = DataFaker.PhysicalDimension.ThermodynamicTemperature.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_Mass = DataFaker.PhysicalDimension.Mass.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_Length = DataFaker.PhysicalDimension.Length.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_AmountOfSubstance = DataFaker.PhysicalDimension.AmountOfSubstance.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_Time = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_ElectricCurrent.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_LuminousIntensity.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_Mass.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_Length.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_Time.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            PhysicalDimensionByFilterOption optFilter = new PhysicalDimensionByFilterOption()
            {
                ConversionFactorToSI = null,
                CultureName = null,
                ExponentOfAmpere = null,
                ExponentOfCandela = null,
                ExponentOfKelvin = null,
                ExponentOfKilogram = null,
                ExponentOfMetre = null,
                ExponentOfMole = null,
                ExponentOfSecond = null,
                Name = null,
                Symbol = null,
                Unit = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.FindByFilterAsync(optFilter, CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                enumPhysicalDimension =>
                {
                    enumPhysicalDimension.Should().NotBeEmpty();

                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_ElectricCurrent.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_LuminousIntensity.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_Mass.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_Length.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_Time.MapToTransferObject());

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_ElectricCurrent.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_LuminousIntensity.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_Mass.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_Length.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_Time.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(1.0f, null, null, null, null, null, null)]
        [InlineData(null, 1.0f, null, null, null, null, null)]
        [InlineData(null, null, 1.0f, null, null, null, null)]
        [InlineData(null, null, null, 1.0f, null, null, null)]
        [InlineData(null, null, null, null, 1.0f, null, null)]
        [InlineData(null, null, null, null, null, 1.0f, null)]
        [InlineData(null, null, null, null, null, null, 1.0f)]
        public async Task FindByFilter_ShouldReturnPhysicalDimension_WhenExponentExists(
            float? fExponentOfAmpere,
            float? fExponentOfCandela,
            float? fExponentOfKelvin,
            float? fExponentOfKilogram,
            float? fExponentOfMetre,
            float? fExponentOfMole,
            float? fExponentOfSecond)
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_ElectricCurrent = DataFaker.PhysicalDimension.ElectricCurrent.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_LuminousIntensity = DataFaker.PhysicalDimension.LuminousIntensity.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_ThermodynamicTemperature = DataFaker.PhysicalDimension.ThermodynamicTemperature.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_Mass = DataFaker.PhysicalDimension.Mass.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_Length = DataFaker.PhysicalDimension.Length.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_AmountOfSubstance = DataFaker.PhysicalDimension.AmountOfSubstance.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_Time = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_ElectricCurrent.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_LuminousIntensity.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_Mass.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_Length.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_Time.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            PhysicalDimensionByFilterOption optFilter = new PhysicalDimensionByFilterOption()
            {
                ConversionFactorToSI = null,
                CultureName = null,
                ExponentOfAmpere = fExponentOfAmpere,
                ExponentOfCandela = fExponentOfCandela,
                ExponentOfKelvin = fExponentOfKelvin,
                ExponentOfKilogram = fExponentOfKilogram,
                ExponentOfMetre = fExponentOfMetre,
                ExponentOfMole = fExponentOfMole,
                ExponentOfSecond = fExponentOfSecond,
                Name = null,
                Symbol = null,
                Unit = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.FindByFilterAsync(optFilter, CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                enumPhysicalDimension =>
                {
                    enumPhysicalDimension.Should().NotBeEmpty();

                    if (fExponentOfAmpere is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_ElectricCurrent.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_ElectricCurrent.MapToTransferObject());

                    if (fExponentOfCandela is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_LuminousIntensity.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_LuminousIntensity.MapToTransferObject());

                    if (fExponentOfKelvin is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject());

                    if (fExponentOfKilogram is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_Mass.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_Mass.MapToTransferObject());

                    if (fExponentOfMetre is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_Length.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_Length.MapToTransferObject());

                    if (fExponentOfMole is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject());

                    if (fExponentOfSecond is not null)
                        enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_Time.MapToTransferObject());
                    else
                        enumPhysicalDimension.Should().NotContainEquivalentOf(pdPhysicalDimension_Time.MapToTransferObject());

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_ElectricCurrent.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_LuminousIntensity.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_ThermodynamicTemperature.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_Mass.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_Length.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_AmountOfSubstance.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_Time.MapToTransferObject(), CancellationToken.None);
        }
    }
}
