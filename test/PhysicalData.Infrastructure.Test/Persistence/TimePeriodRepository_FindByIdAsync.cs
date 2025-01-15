using FluentAssertions;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class TimePeriodRepositorySpecification_FindByIdAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public TimePeriodRepositorySpecification_FindByIdAsync(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task FindById_ShouldReturnTimePeriod_WhenIdExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            RepositoryResult<TimePeriodTransferObject> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                dtoTimePeriodById =>
                {
                    dtoTimePeriodById.Should().BeEquivalentTo(pdTimePeriod);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task FindById_ShouldReturnRepositoryError_WhenIdDoesNotExist()
        {
            // Arrange
            Guid guId = Guid.NewGuid();

            // Act
            RepositoryResult<TimePeriodTransferObject> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(guId, CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TimePeriodError.Code.Method);
                    msgError.Description.Should().Be($"Time period {guId} has not been found.");

                    return false;
                },
                dtoTimePeriod =>
                {
                    dtoTimePeriod.Should().BeNull();

                    return true;
                });
        }
    }
}