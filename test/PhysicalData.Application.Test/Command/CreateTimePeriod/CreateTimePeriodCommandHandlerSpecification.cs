using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.TimePeriod.Create;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Test.Command.CreateTimePeriod
{
    public sealed class CreateTimePeriodCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public CreateTimePeriodCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenTimePeriodIsCreated()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            double dMagnitude = 0.0;

            CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
            {
                PhysicalDimensionId = pdPhysicalDimension.Id,
                Magnitude = new double[] { dMagnitude },
                Offset = 0.0,
                RestrictedPassportId = Guid.NewGuid()
            };

            CreateTimePeriodCommandHandler cmdHandler = new CreateTimePeriodCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<Guid> rsltTimePeriodId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            await rsltTimePeriodId.MatchAsync(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                async guTimePeriodId =>
                {
                    RepositoryResult<TimePeriodTransferObject> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(guTimePeriodId, CancellationToken.None);

                    rsltTimePeriod.Match(
                        msgError =>
                        {
                            msgError.Should().BeNull();

                            return false;
                        },
                        dtoTimePeriod =>
                        {
                            dtoTimePeriod.Magnitude.Should().ContainEquivalentOf(dMagnitude);
                            dtoTimePeriod.Offset.Should().Be(cmdCreate.Offset);

                            return true;
                        });

                    //Clean up
                    await rsltTimePeriod.MatchAsync(
                        msgError => false,
                        async dtoTimePeriod => await fxtPhysicalData.TimePeriodRepository.DeleteAsync(dtoTimePeriod, CancellationToken.None));

                    return true;
                });
        }

        [Fact]
        public async Task Create_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
        {
            // Arrange
            CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            CreateTimePeriodCommandHandler cmdHandler = new CreateTimePeriodCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            IMessageResult<Guid> rsltTimePeriodId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            rsltTimePeriodId.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);

                    return false;
                },
                guTimePeriodId =>
                {
                    guTimePeriodId.Should().BeEmpty();

                    return true;
                });
        }
    }
}