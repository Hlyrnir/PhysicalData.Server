using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Validation;

namespace PhysicalData.Infrastructure.Test
{
    public class PhysicalDataFixture
    {
        private readonly FakeTimeProvider prvTime;

        private readonly IConfiguration cfgConfiguration;

        private readonly IUnitOfWork uowUnitOfWork;

        private readonly IPhysicalDimensionRepository repoPhysicalDimension;
        private readonly ITimePeriodRepository repoTimePeriod;

        public PhysicalDataFixture()
        {
            prvTime = new FakeTimeProvider();
            prvTime.SetUtcNow(new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, 0, TimeSpan.Zero));

            cfgConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new[]
                    {
                        new KeyValuePair<string, string?>("ConnectionStrings:DATABASE_TEST", "Data Source=D:\\Dateien\\Projekte\\CSharp\\PhysicalData.Server\\TEST_PhysicalData.db; Mode=ReadWrite")
                    })
                .Build();

            IDataAccess sqlDataAccess = new SqliteDataAccess(cfgConfiguration, "DATABASE_TEST");

            uowUnitOfWork = new PhysicalData.Infrastructure.UnitOfWork(sqlDataAccess);

            repoPhysicalDimension = new PhysicalData.Infrastructure.Persistence.PhysicalDimensionRepository(sqlDataAccess);
            repoTimePeriod = new PhysicalData.Infrastructure.Persistence.TimePeriodRepository(sqlDataAccess);
        }

        public TimeProvider TimeProvider { get => prvTime; }
        public IMessageValidation MessageValidation { get => new MessageValidation(); }
        public IUnitOfWork UnitOfWork { get => uowUnitOfWork; }
        public IPhysicalDimensionRepository PhysicalDimensionRepository { get => repoPhysicalDimension; }
        public ITimePeriodRepository TimePeriodRepository { get => repoTimePeriod; }
    }
}