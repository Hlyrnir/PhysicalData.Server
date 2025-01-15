using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.TimePeriod.Delete;
using PhysicalData.Application.Extension;

namespace PhysicalData.Application.Test.Command.DeleteTimePeriod
{
    public sealed class DeleteTimePeriodCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public DeleteTimePeriodCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenTimePeriodIsDeleted()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            DeleteTimePeriodCommand cmdDelete = new DeleteTimePeriodCommand()
            {
                TimePeriodId = pdTimePeriod.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            DeleteTimePeriodCommandHandler cmdHandler = new DeleteTimePeriodCommandHandler(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
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
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Delete_ShouldReturnRepositoryError_WhenTimePeriodDoesNotExist()
        {
            // Arrange
            DeleteTimePeriodCommand cmdCreate = new DeleteTimePeriodCommand()
            {
                TimePeriodId = Guid.NewGuid(),
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            DeleteTimePeriodCommandHandler cmdHandler = new DeleteTimePeriodCommandHandler(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            rsltDelete.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.TimePeriod.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.TimePeriod.NotFound.Description);

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