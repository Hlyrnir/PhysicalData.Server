using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.PhysicalDimension.Update;

namespace PhysicalData.Application.Test.Command.UpdatePhysicalDimension
{
    public sealed class UpdatePhysicalDimensionAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public UpdatePhysicalDimensionAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPassportIdIsAuthorized()
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

            IAuthorization<UpdatePhysicalDimensionCommand> hndlAuthorization = new UpdatePhysicalDimensionAuthorization();

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdUpdate,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltAuthorization.Match(
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