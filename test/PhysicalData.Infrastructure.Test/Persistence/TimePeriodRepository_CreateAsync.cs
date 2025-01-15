using FluentAssertions;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class TimePeriodRepositorySpecification_CreateAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public TimePeriodRepositorySpecification_CreateAsync(PhysicalDataFixture fxtAuthorizationData)
        {
            this.fxtPhysicalData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionIsCreated()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            RepositoryResult<bool> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
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

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Create_ShouldReturnRepositoryError_WhenTimePeriodExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            RepositoryResult<bool> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TimePeriodError.Code.Method);
                    msgError.Description.Should().Be($"Time period has not been created.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }
    }
}