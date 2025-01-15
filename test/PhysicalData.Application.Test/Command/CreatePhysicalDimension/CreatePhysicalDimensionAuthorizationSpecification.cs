using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.PhysicalDimension.Create;

namespace PhysicalData.Application.Test.Command.CreatePhysicalDimension
{
    public sealed class CreatePhysicalDimensionAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public CreatePhysicalDimensionAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
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

            IAuthorization<CreatePhysicalDimensionCommand> hndlAuthorization = new CreatePhysicalDimensionAuthorization();

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdCreate,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltAuthorization.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return true;
                },
                bResult =>
                {
                    bResult.Should().BeTrue();

                    return true;
                });
        }
    }
}
