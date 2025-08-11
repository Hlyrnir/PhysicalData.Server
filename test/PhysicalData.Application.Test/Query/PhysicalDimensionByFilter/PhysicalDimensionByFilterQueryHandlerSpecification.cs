using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.PhysicalDimension.ByFilter;

namespace PhysicalData.Application.Test.Query.PhysicalDimensionByFilter
{
    public class PhysicalDimensionByFilterQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public PhysicalDimensionByFilterQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnPhysicalDimension_WhenPhysicalDimensionExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            PhysicalDimensionByFilterQuery qryByFilter = new PhysicalDimensionByFilterQuery()
            {
                Filter = new PhysicalDimensionFilterOption()
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
                    Name = pdPhysicalDimension.Name,
                    Symbol = null,
                    Unit = null,
                    Page = 1,
                    PageSize = 10
                },
                RestrictedPassportId = Guid.Empty
            };

            PhysicalDimensionByFilterQueryHandler hdlQuery = new PhysicalDimensionByFilterQueryHandler(
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<PhysicalDimensionByFilterResult> rsltQuery = await hdlQuery.Handle(qryByFilter, CancellationToken.None);

            //Assert
            rsltQuery.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                rsltPhysicalDimension =>
                {
                    rsltPhysicalDimension.PhysicalDimension.Should().NotBeNull();
                    rsltPhysicalDimension.PhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension.MapToTransferObject());

                    return true;
                });

            //Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }
    }
}