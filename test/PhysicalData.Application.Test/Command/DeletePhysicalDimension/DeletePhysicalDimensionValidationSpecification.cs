using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Command.PhysicalDimension.Delete;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;

namespace PhysicalData.Application.Test.Command.DeletePhysicalDimension
{
    public sealed class DeletePhysicalDimensionValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public DeletePhysicalDimensionValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPhysicalDimensionExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
            {
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.NewGuid(),
            };

            IValidation<DeletePhysicalDimensionCommand> hndlValidation = new DeletePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdDelete,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
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
        public async Task Delete_ShouldReturnMessageError_WhenPhysicalDimensionDoesNotExist()
        {
            // Arrange
            Guid guPhysicalDimensionId = Guid.NewGuid();

            DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
            {
                PhysicalDimensionId = guPhysicalDimensionId,
                RestrictedPassportId = Guid.NewGuid(),
            };

            IValidation<DeletePhysicalDimensionCommand> hndlValidation = new DeletePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdDelete,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain($"Physical dimension {guPhysicalDimensionId} does not exist.");

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