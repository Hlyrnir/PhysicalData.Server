using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.TimePeriod.ByFilter;

namespace PhysicalData.Application.Test.Query.TimePeriodByFilter
{
    public class TimePeriodByFilterQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public TimePeriodByFilterQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnTimePeriod_WhenTimePeriodExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            TimePeriodByFilterQuery qryByFilter = new TimePeriodByFilterQuery()
            {
                Filter = new TimePeriodFilterOption()
                {
                    Magnitude = null,
                    Offset = null,
                    PhysicalDimensionId = pdPhysicalDimension.Id,
                    Page = 1,
                    PageSize = 10
                },
                RestrictedPassportId = Guid.Empty
            };

            TimePeriodByFilterQueryHandler hdlQuery = new TimePeriodByFilterQueryHandler(
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<TimePeriodByFilterResult> rsltQuery = await hdlQuery.Handle(qryByFilter, CancellationToken.None);

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
                    rsltTimePeriod.TimePeriod.Should().ContainEquivalentOf(pdTimePeriod);

                    return true;
                });

            //Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod.MapToTransferObject(), CancellationToken.None);
        }
    }
}