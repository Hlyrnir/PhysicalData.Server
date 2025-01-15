using FluentAssertions;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class PhysicalDimensionRepositorySpecification_FindByIdAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public PhysicalDimensionRepositorySpecification_FindByIdAsync(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task FindById_ShouldReturnPhysicalDimension_WhenIdExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.FindByIdAsync(pdPhysicalDimension.Id, CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                dtoPhysicalDimensionById =>
                {
                    dtoPhysicalDimensionById.Should().BeEquivalentTo(pdPhysicalDimension.MapToTransferObject());

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task FindById_ShouldReturnRepositoryError_WhenIdDoesNotExist()
        {
            // Arrange
            Guid guId = Guid.NewGuid();

            // Act
            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.FindByIdAsync(guId, CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
                    msgError.Description.Should().Be($"Physical dimension {guId} has not been found.");

                    return false;
                },
                dtoPhysicalDimension =>
                {
                    dtoPhysicalDimension.Should().BeNull();

                    return true;
                });
        }
    }
}
