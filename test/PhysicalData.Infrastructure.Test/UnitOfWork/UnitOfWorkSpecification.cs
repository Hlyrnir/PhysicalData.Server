using FluentAssertions;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Infrastructure.Test.UnitOfWork
{
    public class UnitOfWorkSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public UnitOfWorkSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldCreateTimePeriod_WhenTransactionIsCommitted()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            bool bIsCommitted = false;
            bool bIsRolledBack = false;

            // Act
            await fxtPhysicalData.UnitOfWork.TransactionAsync(async () =>
            {
                await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
                await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

                bIsCommitted = fxtPhysicalData.UnitOfWork.TryCommit();

                if (bIsCommitted == false)
                    bIsRolledBack = fxtPhysicalData.UnitOfWork.TryRollback();
            });

            // Assert
            RepositoryResult<TimePeriodTransferObject> rsltPassport = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

            bIsCommitted.Should().BeTrue();
            bIsRolledBack.Should().BeFalse();

            rsltPassport.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                dtoTimePeriodInRepository =>
                {
                    dtoTimePeriodInRepository.Should().BeEquivalentTo(pdTimePeriod);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldNotCreateTimePeriod_WhenTransactionIsRolledBack()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            bool bIsCommitted = false;
            bool bIsRolledBack = false;

            // Act
            await fxtPhysicalData.UnitOfWork.TransactionAsync(async () =>
            {
                await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
                await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

                bIsRolledBack = fxtPhysicalData.UnitOfWork.TryRollback();
            });

            // Assert
            RepositoryResult<TimePeriodTransferObject> rsltPassport = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

            bIsCommitted.Should().BeFalse();
            bIsRolledBack.Should().BeTrue();

            rsltPassport.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TimePeriodError.Code.Method);
                    msgError.Description.Should().Be($"Time period {pdTimePeriod.Id} has not been found.");

                    return false;
                },
                dtoTimePeriodInRepository =>
                {
                    dtoTimePeriodInRepository.Should().BeNull();

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }
    }
}