using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Query.TimePeriod.ById;

namespace PhysicalData.Application.Test.Query.TimePeriodById
{
    public sealed class TimePeriodByIdAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public TimePeriodByIdAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
            {
                TimePeriodId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<TimePeriodByIdQuery> hndlAuthorization = new TimePeriodByIdAuthorization();

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