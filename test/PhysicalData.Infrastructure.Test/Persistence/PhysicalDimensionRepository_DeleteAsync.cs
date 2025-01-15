using FluentAssertions;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class PhysicalDimensionRepositorySpecification_DeleteAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public PhysicalDimensionRepositorySpecification_DeleteAsync(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPhysicalDimensionIsDeleted()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            RepositoryResult<bool> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeTrue();

                    return true;
                });
        }

        [Fact]
        public async Task Delete_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            if (pdPhysicalDimension is null)
                throw new ArgumentNullException();

            // Act
            RepositoryResult<bool> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
                    msgError.Description.Should().Be($"Physical dimension {pdPhysicalDimension.Id} has not been deleted.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }

        [Fact]
        public async Task Delete_ShouldReturnRepositoryError_WhenConcurrencyStampIsNotActual()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            if (pdPhysicalDimension is null)
                throw new ArgumentNullException();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.UpdateAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            RepositoryResult<bool> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);

            // Assert
            rsltPhysicalDimension.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
                    msgError.Description.Should().Be($"Physical dimension {pdPhysicalDimension.Id} has not been deleted.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });

            // Clean up
            RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimensionToDelete = await fxtPhysicalData.PhysicalDimensionRepository.FindByIdAsync(pdPhysicalDimension.Id, CancellationToken.None);

            await rsltPhysicalDimensionToDelete.MatchAsync(
                msgError => false,
                async dtoPhysicalDimensionToDelete =>
                {
                    await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(dtoPhysicalDimensionToDelete, CancellationToken.None);

                    return true;
                });
        }
    }
}