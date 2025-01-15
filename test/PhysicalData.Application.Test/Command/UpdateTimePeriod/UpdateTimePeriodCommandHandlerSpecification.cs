using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.TimePeriod.Update;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;

namespace PhysicalData.Application.Test.Command.UpdateTimePeriod
{
    public sealed class UpdateTimePeriodCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public UpdateTimePeriodCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
        {
            this.fxtPhysicalData = fxtPhysicalDimension;
            this.prvTime = fxtPhysicalDimension.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenTimePeriodIsUpdated()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
            {
                ConcurrencyStamp = pdTimePeriod.ConcurrencyStamp,
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                TimePeriodId = pdTimePeriod.Id
            };

            UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
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
        public async Task Update_ShouldReturnRepositoryError_WhenTimePeriodDoesNotExist()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                TimePeriodId = Guid.NewGuid()
            };

            // Act
            UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
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

                    return false;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenConcurrencyStampDoNotMatch()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            string sObsoleteConcurrencyStamp = Guid.NewGuid().ToString();

            UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
            {
                ConcurrencyStamp = sObsoleteConcurrencyStamp,
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                TimePeriodId = pdTimePeriod.Id
            };

            UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Should().Be(DefaultMessageError.ConcurrencyViolation);

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenTimePeriodIsNotUpdated()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdValidPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdValidPhysicalDimension.Id);
            Domain.Aggregate.PhysicalDimension pdInvalidPhysicalDimension = DataFaker.PhysicalDimension.Length.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdValidPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdInvalidPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
            {
                ConcurrencyStamp = pdTimePeriod.ConcurrencyStamp,
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                PhysicalDimensionId = pdInvalidPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                TimePeriodId = pdTimePeriod.Id
            };

            // Act
            UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(DomainError.Code.Method);
                    msgError.Description.Should().Be("Physical dimension could not be changed.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdValidPhysicalDimension.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdInvalidPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }
    }
}