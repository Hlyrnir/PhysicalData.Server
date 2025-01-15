using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Query.PhysicalDimension.ById;

namespace PhysicalData.Application.Test.Query.PhysicalDimensionById
{
    public sealed class PhysicalDimensionByIdAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public PhysicalDimensionByIdAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<PhysicalDimensionByIdQuery> hndlAuthorization = new PhysicalDimensionByIdAuthorization();

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: qryById,
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