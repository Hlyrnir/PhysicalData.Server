using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.PhysicalDimension.Delete;
using PhysicalData.Application.Extension;

namespace PhysicalData.Application.Test.Command.DeletePhysicalDimension
{
    public sealed class DeletePhysicalDimensionCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public DeletePhysicalDimensionCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
        {
            fxtPhysicalData = fxtPhysicalDimension;
            prvTime = fxtPhysicalDimension.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPhysicalDimensionIsDeleted()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
            {
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            DeletePhysicalDimensionCommandHandler cmdHandler = new DeletePhysicalDimensionCommandHandler(
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

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
        }

        [Fact]
        public async Task Delete_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
        {
            // Arrange
            DeletePhysicalDimensionCommand cmdCreate = new DeletePhysicalDimensionCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            DeletePhysicalDimensionCommandHandler cmdHandler = new DeletePhysicalDimensionCommandHandler(
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            rsltDelete.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);

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