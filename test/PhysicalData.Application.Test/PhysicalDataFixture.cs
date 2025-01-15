using Microsoft.Extensions.Time.Testing;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Test.Fake;
using PhysicalData.Application.Test.Fake.Repository;
using PhysicalData.Application.Validation;

namespace PhysicalData.Application.Test
{
    public class PhysicalDataFixture
    {
        private readonly FakeTimeProvider prvTime;

        private readonly IUnitOfWork uowUnitOfWork;

        private readonly IPhysicalDimensionRepository repoPhysicalDimension;
        private readonly ITimePeriodRepository repoTimePeriod;

        public PhysicalDataFixture()
        {
            prvTime = new FakeTimeProvider();
            prvTime.SetUtcNow(new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, 0, TimeSpan.Zero));

            uowUnitOfWork = new FakeUnitOfWork();

            FakeDatabase dbFake = new FakeDatabase();

            repoPhysicalDimension = new FakePhysicalDimensionRepository(dbFake);
            repoTimePeriod = new FakeTimePeriodRepository(dbFake);
        }

        public TimeProvider TimeProvider { get => prvTime; }
        public IMessageValidation MessageValidation { get => new MessageValidation(); }
        public IUnitOfWork UnitOfWork { get => uowUnitOfWork; }
        public IPhysicalDimensionRepository PhysicalDimensionRepository { get => repoPhysicalDimension; }
        public ITimePeriodRepository TimePeriodRepository { get => repoTimePeriod; }
    }
}