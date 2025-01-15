﻿using FluentAssertions;
using Passport.Abstraction.Result;
using Passport.Abstraction.Validation;
using PhysicalData.Application.Command.TimePeriod.Create;

namespace PhysicalData.Application.Test.Command.CreateTimePeriod
{
    public sealed class CreateTimePeriodValidationSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtPhysicalData;
        private readonly TimeProvider prvTime;

        public CreateTimePeriodValidationSpecification(PhysicalDataFixture fxtPhysicalData)
        {
            this.fxtPhysicalData = fxtPhysicalData;
            this.prvTime = fxtPhysicalData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenTimePeriodDoesNotExist()
        {
            // Arrange
            CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                Magnitude = new double[] { 0.0 },
                Offset = 0.0,
                RestrictedPassportId = Guid.NewGuid(),
            };

            IValidation<CreateTimePeriodCommand> hndlValidation = new CreateTimePeriodValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdCreate,
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

        [Theory]
        [InlineData(true, new double[] { double.MinValue, double.MinValue })]
        [InlineData(true, new double[] { double.MaxValue, double.MaxValue })]
        public async Task Create_ShouldReturnTrue_WhenMagnitudeIsValid(bool bExpectedResult, double[] dMagnitude)
        {
            // Arrange
            CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                Magnitude = dMagnitude,
                Offset = 0.0,
                RestrictedPassportId = Guid.NewGuid(),
            };

            IValidation<CreateTimePeriodCommand> hndlValidation = new CreateTimePeriodValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdCreate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });
        }

        [Theory]
        [InlineData(true, double.MinValue)]
        [InlineData(true, double.MaxValue)]
        public async Task Create_ShouldReturnTrue_WhenOffsetIsValid(bool bExpectedResult, double dOffset)
        {
            // Arrange
            CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
            {
                PhysicalDimensionId = Guid.NewGuid(),
                Magnitude = new double[] { 0.0 },
                Offset = dOffset,
                RestrictedPassportId = Guid.NewGuid(),
            };

            IValidation<CreateTimePeriodCommand> hndlValidation = new CreateTimePeriodValidation(
                srvValidation: fxtPhysicalData.MessageValidation,
                repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdCreate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });
        }
    }
}