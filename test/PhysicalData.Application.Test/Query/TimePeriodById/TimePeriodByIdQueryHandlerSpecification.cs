using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Query.TimePeriod.ById;

namespace PhysicalData.Application.Test.Query.TimePeriodById
{
    public class TimePeriodByIdQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public TimePeriodByIdQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTimePeriod_WhenPeriodExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
            {
                RestrictedPassportId = Guid.NewGuid(),
                TimePeriodId = pdTimePeriod.Id
            };

            TimePeriodByIdQueryHandler hdlQuery = new TimePeriodByIdQueryHandler(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<TimePeriodByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

            //Assert
            rsltQuery.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                rsltTimePeriod =>
                {
                    rsltTimePeriod.TimePeriod.Should().NotBeNull();
                    rsltTimePeriod.TimePeriod.Should().BeEquivalentTo(pdTimePeriod.MapToTransferObject());

                    return true;
                });

            //Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Read_ShouldReturnMessageError_WhenTimePeriodDoesNotExist()
        {
            // Arrange
            TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
            {
                RestrictedPassportId = Guid.NewGuid(),
                TimePeriodId = Guid.NewGuid()
            };

            TimePeriodByIdQueryHandler hdlQuery = new TimePeriodByIdQueryHandler(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<TimePeriodByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

            //Assert
            rsltQuery.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.TimePeriod.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.TimePeriod.NotFound.Description);

                    return false;
                },
                rsltTimePeriod =>
                {
                    rsltTimePeriod.Should().BeNull();

                    return true;
                });
        }
    }
}