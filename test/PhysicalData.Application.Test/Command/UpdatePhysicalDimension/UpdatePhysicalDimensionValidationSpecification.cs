using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Command.PhysicalDimension.Update;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;

namespace PhysicalData.Application.Test.Command.UpdatePhysicalDimension
{
    public sealed class UpdatePhysicalDimensionValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public UpdatePhysicalDimensionValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPhysicalDimensionExists()
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

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
        public async Task Update_ShouldReturnMessageError_WhenPhysicalDimensionDoesNotExist()
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

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().ContainEquivalentOf($"Physical dimension {cmdUpdate.PhysicalDimensionId} does not exist.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfAmpereIsValid(bool bExpectedResult, float fExponentOfAmpere)
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            
            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = fExponentOfAmpere,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 0,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Electric current",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "I",
                Unit = "A"
            };

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfCandelaIsValid(bool bExpectedResult, float fExponentOfCandela)
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = fExponentOfCandela,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 0,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Luminous intensity",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "J",
                Unit = "cd"
            };

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfKelvinIsValid(bool bExpectedResult, float fExponentOfKelvin)
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = fExponentOfKelvin,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 0,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Thermodynamical temperature",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "T",
                Unit = "K"
            };

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfKilogramIsValid(bool bExpectedResult, float fExponentOfKilogram)
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePhysicalDimensionCommand cmdUpdate = new UpdatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = fExponentOfKilogram,
                ExponentOfMetre = 0,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Mass",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "m",
                Unit = "kg"
            };

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfMetreIsValid(bool bExpectedResult, float fExponentOfMetre)
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
                ExponentOfMetre = fExponentOfMetre,
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

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfMoleIsValid(bool bExpectedResult, float fExponentOfMole)
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
                ExponentOfMetre = 0,
                ExponentOfMole = fExponentOfMole,
                ExponentOfSecond = 0,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Amount of substance",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "N",
                Unit = "mol"
            };

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, float.MaxValue)]
        [InlineData(true, float.MinValue)]
        public async Task Update_ShouldReturnTrue_WhenExponentOfSecondIsValid(bool bExpectedResult, float fExponentOfSecond)
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
                ExponentOfMetre = 0,
                ExponentOfMole = 0,
                ExponentOfSecond = fExponentOfSecond,
                ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Amount of substance",
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.Empty,
                Symbol = "N",
                Unit = "mol"
            };

            IValidation<UpdatePhysicalDimensionCommand> hndlValidation = new UpdatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }
    }
}
