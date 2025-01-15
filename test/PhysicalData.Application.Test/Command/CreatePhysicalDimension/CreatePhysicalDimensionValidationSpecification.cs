using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Command.PhysicalDimension.Create;

namespace PhysicalData.Application.Test.Command.CreatePhysicalDimension
{
    public sealed class CreatePhysicalDimensionValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public CreatePhysicalDimensionValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionDoesNotExist()
        {
            CreatePhysicalDimensionCommand cmdCreate = new CreatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 1,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Metre",
                Symbol = "l",
                Unit = "m",
                RestrictedPassportId = Guid.Empty
            };

            IValidation<CreatePhysicalDimensionCommand> hndlValidation = new CreatePhysicalDimensionValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdCreate,
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
        }
    }
}