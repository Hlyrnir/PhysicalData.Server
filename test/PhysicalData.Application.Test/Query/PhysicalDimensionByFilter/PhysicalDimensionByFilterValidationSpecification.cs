using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.PhysicalDimension.ByFilter;

namespace PhysicalData.Application.Test.Query.PhysicalDimensionByFilter
{
    public sealed class PhysicalDimensionByFilterValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public PhysicalDimensionByFilterValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenFilterIsValid()
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

            IValidation<PhysicalDimensionByFilterQuery> hndlValidation = new PhysicalDimensionByFilterValidation(
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

        [Fact]
        public async Task Read_ShouldReturnMessageError_WhenFilterContainsSqlStatement()
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
                    Name = "SELECT",
                    Symbol = null,
                    Unit = null,
                    Page = 1,
                    PageSize = 10
                },
                RestrictedPassportId = Guid.Empty
            };

            IValidation<PhysicalDimensionByFilterQuery> hndlValidation = new PhysicalDimensionByFilterValidation(
                srvValidation: fxtPhysicalData.MessageValidation);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: qryByFilter,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain("Name contains forbidden statement.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }
    }
}