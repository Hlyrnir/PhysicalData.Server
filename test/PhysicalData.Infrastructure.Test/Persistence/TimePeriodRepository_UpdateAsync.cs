using FluentAssertions;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class TimePeriodRepositorySpecification_UpdateAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public TimePeriodRepositorySpecification_UpdateAsync(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldUpdateTimePeriod_WhenTimePeriodExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            pdTimePeriod.Offset = 100.0;

            RepositoryResult<bool> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.UpdateAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

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
            RepositoryResult<TimePeriodTransferObject> rsltTimePeriodToDelete = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

            await rsltTimePeriodToDelete.MatchAsync(
                msgError => false,
                async dtoTimePeriodToDelete =>
                {
                    await fxtPhysicalData.TimePeriodRepository.DeleteAsync(dtoTimePeriodToDelete, CancellationToken.None);

                    return true;
                });

            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenTimePeriodDoesNotExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            // Act
            RepositoryResult<bool> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.UpdateAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TimePeriodError.Code.Method);
                    msgError.Description.Should().Be($"Time period {pdTimePeriod.Id} has not been updated.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }
    }
}