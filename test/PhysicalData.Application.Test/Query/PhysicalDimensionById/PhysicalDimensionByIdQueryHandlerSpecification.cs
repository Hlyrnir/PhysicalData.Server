using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Query.PhysicalDimension.ById;

namespace PhysicalData.Application.Test.Query.PhysicalDimensionById
{
    public class PhysicalDimensionByIdQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public PhysicalDimensionByIdQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Read_ShouldReturnPhysicalDimension_WhenPhysicalDimensionExists()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
            {
                PhysicalDimensionId = pdPhysicalDimension.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            PhysicalDimensionByIdQueryHandler hdlQuery = new PhysicalDimensionByIdQueryHandler(
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<PhysicalDimensionByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

            //Assert
            rsltQuery.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                rsltPhysicalDimension =>
                {
                    rsltPhysicalDimension.PhysicalDimension.Should().NotBeNull();
                    rsltPhysicalDimension.PhysicalDimension.Should().BeEquivalentTo(pdPhysicalDimension.MapToTransferObject());

                    return true;
                });

            //Clean up
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task Read_ShouldReturnMessageError_WhenPhysicalDimensionDoesNotExist()
        {
            // Arrange
            PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                RestrictedPassportId = Guid.NewGuid()
            };

            PhysicalDimensionByIdQueryHandler hdlQuery = new PhysicalDimensionByIdQueryHandler(
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<PhysicalDimensionByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

            //Assert
            rsltQuery.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);

                    return false;
                },
                rsltPhysicalDimension =>
                {
                    rsltPhysicalDimension.Should().BeNull();

                    return true;
                });
        }
    }
}