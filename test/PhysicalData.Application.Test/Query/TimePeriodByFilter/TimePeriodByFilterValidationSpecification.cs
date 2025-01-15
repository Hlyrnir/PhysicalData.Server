using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.TimePeriod.ByFilter;

namespace PhysicalData.Application.Test.Query.TimePeriodByFilter
{
    public sealed class TimePeriodByFilterValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public TimePeriodByFilterValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenFilterIsValid()
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

            IValidation<TimePeriodByFilterQuery> hndlValidation = new TimePeriodByFilterValidation(
                srvValidation: fxtPhysicalData.MessageValidation);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: qryByFilter,
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