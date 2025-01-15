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
        public async Task FindByFilter_ShouldReturnPhysicalDimension_WhenIdExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_01 = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension_02 = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_01.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_02.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

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

                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_01.MapToTransferObject());
                    enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_02.MapToTransferObject());

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_01.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_02.MapToTransferObject(), CancellationToken.None);
        }
    }
}
