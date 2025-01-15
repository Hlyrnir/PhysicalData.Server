using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.TimePeriod.Create;

namespace PhysicalData.Application.Test.Command.CreateTimePeriod
{
    public sealed class CreateTimePeriodAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public CreateTimePeriodAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<CreateTimePeriodCommand> hndlAuthorization = new CreateTimePeriodAuthorization();

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