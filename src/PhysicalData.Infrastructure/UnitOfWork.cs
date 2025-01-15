using Microsoft.Extensions.DependencyInjection;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;

namespace PhysicalData.Infrastructure
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDataAccess sqlDataAccess;

        public UnitOfWork([FromKeyedServices(DefaultKeyedServiceName.DataAccess)] IDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }

        /// <inheritdoc/>
        public async Task TransactionAsync(Func<Task> MethodForTransaction)
        {
            await sqlDataAccess.TransactionAsync(MethodForTransaction);
        }

        /// <inheritdoc/>
        public bool TryCommit()
        {
            return sqlDataAccess.TryCommit();
        }

        /// <inheritdoc/>
        public bool TryRollback()
        {
            return sqlDataAccess.TryRollback();
        }
    }
}
