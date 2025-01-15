using FluentAssertions;
using Passport.Abstraction.Result;
using PhysicalData.Application.Command.PhysicalDimension.Create;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Test.Command.CreatePhysicalDimension
{
    public sealed class CreatePhysicalDimensionCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public CreatePhysicalDimensionCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
        {
            fxtPhysicalData = fxtPhysicalDimension;
            prvTime = fxtPhysicalDimension.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionIsCreated()
        {
            // Arrange
            CreatePhysicalDimensionCommand cmdCreate = new CreatePhysicalDimensionCommand()
            {
                ExponentOfAmpere = 0,
                ExponentOfCandela = 0,
                ExponentOfKelvin = 0,
                ExponentOfKilogram = 0,
                ExponentOfMetre = 1,
                ExponentOfMole = 0,
                ExponentOfSecond = 0,
                ConversionFactorToSI = 1,
                CultureName = "en-GB",
                Name = "Metre",
                Symbol = "l",
                Unit = "m",
                RestrictedPassportId = Guid.Empty
            };

            CreatePhysicalDimensionCommandHandler cmdHandler = new CreatePhysicalDimensionCommandHandler(
                prvTime: prvTime,
                repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

            // Act
            IMessageResult<Guid> rsltPhysicalDimensionId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            await rsltPhysicalDimensionId.MatchAsync(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                async guPhysicalDimensionId =>
                {
                    RepositoryResult<PhysicalDimensionTransferObject> rsltPhysicalDimension = await fxtPhysicalData.PhysicalDimensionRepository.FindByIdAsync(guPhysicalDimensionId, CancellationToken.None);

                    rsltPhysicalDimension.Match(
                        msgError =>
                        {
                            msgError.Should().BeNull();

                            return false;
                        },
                        pdPhysicalDimension =>
                        {
                            pdPhysicalDimension.ExponentOfAmpere.Should().Be(cmdCreate.ExponentOfAmpere);
                            pdPhysicalDimension.ExponentOfCandela.Should().Be(cmdCreate.ExponentOfCandela);
                            pdPhysicalDimension.ExponentOfKelvin.Should().Be(cmdCreate.ExponentOfKelvin);
                            pdPhysicalDimension.ExponentOfKilogram.Should().Be(cmdCreate.ExponentOfKilogram);
                            pdPhysicalDimension.ExponentOfMetre.Should().Be(cmdCreate.ExponentOfMetre);
                            pdPhysicalDimension.ExponentOfMole.Should().Be(cmdCreate.ExponentOfMole);
                            pdPhysicalDimension.ExponentOfSecond.Should().Be(cmdCreate.ExponentOfSecond);
                            pdPhysicalDimension.ConversionFactorToSI.Should().Be(cmdCreate.ConversionFactorToSI);
                            pdPhysicalDimension.CultureName.Should().Be(cmdCreate.CultureName);
                            pdPhysicalDimension.Name.Should().Be(cmdCreate.Name);
                            pdPhysicalDimension.Symbol.Should().Be(cmdCreate.Symbol);
                            pdPhysicalDimension.Unit.Should().Be(cmdCreate.Unit);

                            return true;
                        });

                    //Clean up
                    await rsltPhysicalDimension.MatchAsync(
                        msgError => false,
                        async pdPhysicalDimension => await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None));

                    return true;
                });
        }
    }
}