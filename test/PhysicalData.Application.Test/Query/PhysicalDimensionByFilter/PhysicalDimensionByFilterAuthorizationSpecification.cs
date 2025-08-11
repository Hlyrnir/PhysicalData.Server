using FluentAssertions;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.PhysicalDimension.ByFilter;

namespace PhysicalData.Application.Test.Query.PhysicalDimensionByFilter
{
    public sealed class PhysicalDimensionByFilterAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public PhysicalDimensionByFilterAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            PhysicalDimensionByFilterQuery qryByFilter = new PhysicalDimensionByFilterQuery()
            {
                Filter = new PhysicalDimensionFilterOption()
                {
                    ConversionFactorToSI = null,
                    CultureName = null,
                    ExponentOfAmpere = null,
                    ExponentOfCandela = null,
                    ExponentOfKelvin = null,
                    ExponentOfKilogram = null,
                    ExponentOfMetre = null,
                    ExponentOfMole = null,
                    ExponentOfSecond = null,
                    Name = null,
                    Symbol = null,
                    Unit = null,
                    Page = 1,
                    PageSize = 10
                },
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<PhysicalDimensionByFilterQuery> hndlAuthorization = new PhysicalDimensionByFilterAuthorization();

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