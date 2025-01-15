namespace PhysicalData.Api
{
    public static class PhysicalDataServiceCollectionExtension
    {
        public static PhysicalDataServiceCollectionBuilder AddSqliteDatabase(this PhysicalDataServiceCollectionBuilder cltService, string sConnectionStringName)
        {
            PhysicalData.Infrastructure.ServiceCollectionExtension.AddSqliteDatabase(cltService.Services, sConnectionStringName);

            return cltService;
        }
    }
}
