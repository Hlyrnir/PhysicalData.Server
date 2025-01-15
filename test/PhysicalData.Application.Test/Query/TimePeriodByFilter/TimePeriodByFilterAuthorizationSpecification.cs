using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.TimePeriod.ByFilter;

namespace PhysicalData.Application.Test.Query.TimePeriodByFilter
{
    public sealed class TimePeriodByFilterAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public TimePeriodByFilterAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            TimePeriodByFilterQuery qryByFilter = new TimePeriodByFilterQuery()
            {
                Filter = new TimePeriodByFilterOption()
                {
                    Magnitude = null,
                    Offset = null,
                    PhysicalDimensionId = null,
                    Page = 1,
                    PageSize = 10
                },
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<TimePeriodByFilterQuery> hndlAuthorization = new TimePeriodByFilterAuthorization();

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: qryByFilter,
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