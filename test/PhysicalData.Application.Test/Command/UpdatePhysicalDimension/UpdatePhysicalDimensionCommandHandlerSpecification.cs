using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.PhysicalDimension.Update;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;

namespace PhysicalData.Application.Test.Command.UpdatePhysicalDimension
{
    public sealed class UpdatePhysicalDimensionCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public UpdatePhysicalDimensionCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPhysicalDimensionIsUpdated()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 1,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Metre",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "l",
                Unit = "m"
            };

            UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

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
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
        {
            // Arrange
            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 1,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Metre",
                PhysicalDimensionId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty,
                Symbol = "l",
                Unit = "m"
            };

            // Act
            UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
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

                    return false;
                });
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenConcurrencyStampDoNotMatch()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            string sObsoleteConcurrencyStamp = Guid.NewGuid().ToString();

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 1,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = sObsoleteConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Metre",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "l",
                Unit = "m"
            };

            UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

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
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenCultureNameIsNotValid()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            string sInvalidCultureName = "INVALID_CULTURE_NAME";

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 1,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = sInvalidCultureName,
                Name = "Metre",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "l",
                Unit = "m"
            };

            // Act
            UpdatePhysicalDimensionCommandHandler cmdHandler = new UpdatePhysicalDimensionCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(DomainError.Code.Method);
                    msgError.Description.Should().Be("Culture name is not valid.");

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
    }
}