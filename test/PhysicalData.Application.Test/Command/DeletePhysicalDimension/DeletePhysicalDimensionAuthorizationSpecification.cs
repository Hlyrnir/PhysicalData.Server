using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.PhysicalDimension.Delete;

namespace PhysicalData.Application.Test.Command.DeletePhysicalDimension
{
    public sealed class DeletePhysicalDimensionAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public DeletePhysicalDimensionAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<DeletePhysicalDimensionCommand> hndlAuthorization = new DeletePhysicalDimensionAuthorization();

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
