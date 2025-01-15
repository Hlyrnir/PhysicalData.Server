using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.TimePeriod.Delete;

namespace PhysicalData.Application.Test.Command.DeleteTimePeriod
{
    public sealed class DeleteTimePeriodAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public DeleteTimePeriodAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            DeleteTimePeriodCommand cmdDelete = new DeleteTimePeriodCommand()
            {
                TimePeriodId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<DeleteTimePeriodCommand> hndlAuthorization = new DeleteTimePeriodAuthorization();

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdDelete,
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