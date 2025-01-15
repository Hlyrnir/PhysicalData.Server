using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.TimePeriod.Update;

namespace PhysicalData.Application.Test.Command.UpdateTimePeriod
{
    public sealed class UpdateTimePeriodAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public UpdateTimePeriodAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                PhysicalDimensionId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty,
                TimePeriodId = Guid.NewGuid(),
            };

            IAuthorization<UpdateTimePeriodCommand> hndlAuthorization = new UpdateTimePeriodAuthorization();

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