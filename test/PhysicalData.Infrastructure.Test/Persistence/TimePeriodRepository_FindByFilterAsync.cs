using FluentAssertions;
using PhysicalData.Application.Extension;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;
using System.Text.Json;

namespace PhysicalData.Infrastructure.Test.Persistence
{
    public class TimePeriodRepositorySpecification_FindByFilterAsync : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;

        private readonly TimeProvider prvTime;

        public TimePeriodRepositorySpecification_FindByFilterAsync(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task FindByFilter_ShouldReturnTimePeriod_WhenFilterIsNull()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod_01 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_02 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_03 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_04 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_01.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_02.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_03.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_04.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            TimePeriodFilterOption optFilter = new TimePeriodFilterOption()
            {
                Magnitude = null,
                Offset = null,
                PhysicalDimensionId = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            RepositoryResult<IEnumerable<TimePeriodTransferObject>> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByFilterAsync(optFilter, CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                enumTimePeriod =>
                {
                    enumTimePeriod.Should().NotBeEmpty();
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_01);
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_02);
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_03);
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_04);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_01.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_02.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_03.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_04.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task FindByFilter_ShouldReturnTimePeriod_WhenMagnitudeIsFiltered()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod_01 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_02 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_03 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_04 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_01.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_02.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_03.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_04.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            string sMagnitude = JsonSerializer.Serialize(pdTimePeriod_01.Magnitude);

            TimePeriodFilterOption optFilter = new TimePeriodFilterOption()
            {
                Magnitude = sMagnitude,
                Offset = null,
                PhysicalDimensionId = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            RepositoryResult<IEnumerable<TimePeriodTransferObject>> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByFilterAsync(optFilter, CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                enumTimePeriod =>
                {
                    enumTimePeriod.Should().NotBeEmpty();
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_01);
                    enumTimePeriod.Should().NotContainEquivalentOf(pdTimePeriod_02);
                    enumTimePeriod.Should().NotContainEquivalentOf(pdTimePeriod_03);
                    enumTimePeriod.Should().NotContainEquivalentOf(pdTimePeriod_04);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_01.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_02.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_03.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_04.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task FindByFilter_ShouldReturnTimePeriod_WhenOffsetIsFiltered()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod_01 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_02 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_03 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_04 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_01.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_02.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_03.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_04.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            double dOffset = pdTimePeriod_01.Offset;

            TimePeriodFilterOption optFilter = new TimePeriodFilterOption()
            {
                Magnitude = null,
                Offset = dOffset,
                PhysicalDimensionId = null,
                Page = 1,
                PageSize = 10
            };

            // Act
            RepositoryResult<IEnumerable<TimePeriodTransferObject>> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByFilterAsync(optFilter, CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                enumTimePeriod =>
                {
                    enumTimePeriod.Should().NotBeEmpty();
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_01);
                    enumTimePeriod.Should().NotContainEquivalentOf(pdTimePeriod_02);
                    enumTimePeriod.Should().NotContainEquivalentOf(pdTimePeriod_03);
                    enumTimePeriod.Should().NotContainEquivalentOf(pdTimePeriod_04);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_01.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_02.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_03.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_04.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }

        [Fact]
        public async Task FindByFilter_ShouldReturnTimePeriod_WhenPhysicalDimensionIdIsFiltered()
        {
            // Arrange
            Domain.Aggregate.PhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Time.CreateDefault();
            Domain.Aggregate.TimePeriod pdTimePeriod_01 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_02 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_03 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);
            Domain.Aggregate.TimePeriod pdTimePeriod_04 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension.Id);

            await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_01.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_02.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_03.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod_04.MapToTransferObject(), prvTime.GetUtcNow(), CancellationToken.None);

            Guid guPhysicalDimensionId = pdTimePeriod_01.PhysicalDimensionId;

            TimePeriodFilterOption optFilter = new TimePeriodFilterOption()
            {
                Magnitude = null,
                Offset = null,
                PhysicalDimensionId = guPhysicalDimensionId,
                Page = 1,
                PageSize = 10
            };

            // Act
            RepositoryResult<IEnumerable<TimePeriodTransferObject>> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByFilterAsync(optFilter, CancellationToken.None);

            // Assert
            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                enumTimePeriod =>
                {
                    enumTimePeriod.Should().NotBeEmpty();
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_01);
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_02);
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_03);
                    enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_04);

                    return true;
                });

            // Clean up
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_01.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_02.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_03.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod_04.MapToTransferObject(), CancellationToken.None);
            await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension.MapToTransferObject(), CancellationToken.None);
        }
    }
}