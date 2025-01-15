using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Default;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Query.TimePeriod.ById;

namespace PhysicalData.Application.Test.Query.TimePeriodById
{
    public sealed class TimePeriodByIdValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public TimePeriodByIdValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTrue_WhenTimePeriodIdExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
            {
                TimePeriodId = pdTimePeriod.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<TimePeriodByIdQuery> hndlValidation = new TimePeriodByIdValidation(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository,
                srvValidation: fxtPhysicalData.MessageValidation,
                prvTime: fxtPhysicalData.TimeProvider);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: qryById,
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

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Read_ShouldReturnMessageError_WhenTimePeriodDoesNotExist()
        {
            // Arrange
            Guid guTimePeriodId = Guid.NewGuid();

            TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
            {
                TimePeriodId = guTimePeriodId,
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<TimePeriodByIdQuery> hndlValidation = new TimePeriodByIdValidation(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository,
                srvValidation: fxtPhysicalData.MessageValidation,
                prvTime: fxtPhysicalData.TimeProvider);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: qryById,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain($"Time period {guTimePeriodId} does not exist.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }

        [Fact]
        public async Task Read_ShouldReturnMessageError_WhenTimePeriodIdIsEmpty()
        {
            // Arrange
            TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
            {
                TimePeriodId = Guid.Empty,
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<TimePeriodByIdQuery> hndlValidation = new TimePeriodByIdValidation(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository,
                srvValidation: fxtPhysicalData.MessageValidation,
                prvTime: fxtPhysicalData.TimeProvider);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: qryById,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain($"Time period identifier is invalid (empty).");

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