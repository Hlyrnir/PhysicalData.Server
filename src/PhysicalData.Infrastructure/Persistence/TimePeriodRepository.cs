using Dapper;
using Microsoft.Extensions.DependencyInjection;
using PhysicalData.Application.Default;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;
using PhysicalData.Infrastructure.Extension;
using PhysicalData.Infrastructure.TypeHandler;

namespace PhysicalData.Infrastructure.Persistence
{
    internal sealed class TimePeriodRepository : ITimePeriodRepository
    {
        private readonly IDataAccess sqlDataAccess;

        public TimePeriodRepository([FromKeyedServices(DefaultKeyedServiceName.DataAccess)] IDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;

            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(typeof(double[]), new DoubleArrayTypeHandler());
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> DeleteAsync(TimePeriodTransferObject dtoTimePeriod, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"DELETE FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{TimePeriodColumn.Id}] = @Id";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ActualStamp", dtoTimePeriod.ConcurrencyStamp);
                dpParameter.Add("Id", dtoTimePeriod.Id);

                int iResult = -1;

                iResult = await sqlDataAccess.Connection.ExecuteAsync(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (iResult < 1)
                    return new RepositoryResult<bool>(new RepositoryError()
                    {
                        Code = TimePeriodError.Code.Method,
                        Description = $"Time period {dtoTimePeriod.Id} has not been deleted."
                    });

                return new RepositoryResult<bool>(true);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> ExistsAsync(
            Guid guTimePeriodId,
            CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("Id", guTimePeriodId);

                bool bResult = await sqlDataAccess.Connection.ExecuteScalarAsync<bool>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                return new RepositoryResult<bool>(bResult);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<IEnumerable<TimePeriodTransferObject>>> FindByFilterAsync(TimePeriodByFilterOption optFilter, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<IEnumerable<TimePeriodTransferObject>>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT 
									[{TimePeriodColumn.ConcurrencyStamp}], 
									[{TimePeriodColumn.Id}], 
									[{TimePeriodColumn.Magnitude}], 
									[{TimePeriodColumn.Offset}], 
									[{TimePeriodColumn.PhysicalDimensionId}] 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE (@Magnitude IS NULL OR [{TimePeriodColumn.Magnitude}] LIKE('%'|| @Magnitude ||'%')) 
                                    AND (@Offset IS NULL OR [{TimePeriodColumn.Offset}] = @Offset) 
                                    AND (@PhysicalDimensionId IS NULL OR [{TimePeriodColumn.PhysicalDimensionId}] = @PhysicalDimensionId) 
									LIMIT @PageSize 
									OFFSET @PageOffset;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("PhysicalDimensionId", optFilter.PhysicalDimensionId);
                dpParameter.Add("Magnitude", optFilter.Magnitude);
                dpParameter.Add("Offset", optFilter.Offset);
                dpParameter.Add("PageOffset", (optFilter.Page + -1) * optFilter.PageSize);
                dpParameter.Add("PageSize", optFilter.PageSize);

                IEnumerable<TimePeriodTransferObject> enumTimePeriodTransferObject = await sqlDataAccess.Connection.QueryAsync<TimePeriodTransferObject>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                return new RepositoryResult<IEnumerable<TimePeriodTransferObject>>(enumTimePeriodTransferObject);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<IEnumerable<TimePeriodTransferObject>>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<TimePeriodTransferObject>> FindByIdAsync(Guid guTimePeriodId, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<TimePeriodTransferObject>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT 
									[{TimePeriodColumn.ConcurrencyStamp}], 
									[{TimePeriodColumn.Id}], 
									[{TimePeriodColumn.Magnitude}], 
									[{TimePeriodColumn.Offset}], 
									[{TimePeriodColumn.PhysicalDimensionId}] 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.Id}] = @Id;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("Id", guTimePeriodId);

                IEnumerable<TimePeriodTransferObject> enumTimePeriodTransferObject = await sqlDataAccess.Connection.QueryAsync<TimePeriodTransferObject>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (enumTimePeriodTransferObject.Count() == 0)
                    return new RepositoryResult<TimePeriodTransferObject>(new RepositoryError()
                    {
                        Code = TimePeriodError.Code.Method,
                        Description = $"Time period {guTimePeriodId} has not been found."
                    });

                return new RepositoryResult<TimePeriodTransferObject>(enumTimePeriodTransferObject.First());
            }
            catch (Exception exException)
            {
                return new RepositoryResult<TimePeriodTransferObject>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> InsertAsync(TimePeriodTransferObject dtoTimePeriod, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"INSERT INTO [{TimePeriodTable.TimePeriod}] (
									[{TimePeriodColumn.ConcurrencyStamp}], 
									[{TimePeriodColumn.Id}], 
									[{TimePeriodColumn.Magnitude}], 
									[{TimePeriodColumn.Offset}], 
									[{TimePeriodColumn.PhysicalDimensionId}]) 
									SELECT 
									@ConcurrencyStamp, 
									@Id, 
									@Magnitude, 
									@Offset, 
									@PhysicalDimensionId
									WHERE NOT EXISTS(
									SELECT 1 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.Id}] = @Id);";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ConcurrencyStamp", dtoTimePeriod.ConcurrencyStamp);
                dpParameter.Add("Id", dtoTimePeriod.Id);
                dpParameter.Add("Magnitude", dtoTimePeriod.Magnitude);
                dpParameter.Add("Offset", dtoTimePeriod.Offset);
                dpParameter.Add("PhysicalDimensionId", dtoTimePeriod.PhysicalDimensionId);

                int iResult = -1;

                iResult = await sqlDataAccess.Connection.ExecuteAsync(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (iResult < 1)
                    return new RepositoryResult<bool>(new RepositoryError()
                    {
                        Code = TimePeriodError.Code.Method,
                        Description = $"Time period has not been created."
                    });

                return new RepositoryResult<bool>(true);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<int>> QuantityByFilterAsync(TimePeriodByFilterOption optFilter, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<int>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT 
									COUNT([{TimePeriodColumn.Id}]) 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE (@Magnitude IS NULL OR [{TimePeriodColumn.Magnitude}] LIKE('%'|| @Magnitude ||'%')) 
                                    AND (@Offset IS NULL OR [{TimePeriodColumn.Offset}] = @Offset) 
                                    AND (@PhysicalDimensionId IS NULL OR [{TimePeriodColumn.PhysicalDimensionId}] = @PhysicalDimensionId);";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("Magnitude", optFilter.Magnitude);
                dpParameter.Add("Offset", optFilter.Offset);
                dpParameter.Add("PhysicalDimensionId", optFilter.PhysicalDimensionId);

                int iQuantity = 0;

                iQuantity = await sqlDataAccess.Connection.QuerySingleAsync<int>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                return new RepositoryResult<int>(iQuantity);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<int>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> UpdateAsync(TimePeriodTransferObject dtoTimePeriod, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"UPDATE [{TimePeriodTable.TimePeriod}] SET 
									[{TimePeriodColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{TimePeriodColumn.Id}] = @Id, 
									[{TimePeriodColumn.Magnitude}] = @Magnitude, 
									[{TimePeriodColumn.Offset}] = @Offset, 
									[{TimePeriodColumn.PhysicalDimensionId}] = @PhysicalDimensionId 
									WHERE [{TimePeriodColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{TimePeriodColumn.Id}] = @Id;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ActualStamp", dtoTimePeriod.ConcurrencyStamp);
                dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
                dpParameter.Add("Id", dtoTimePeriod.Id);
                dpParameter.Add("Magnitude", dtoTimePeriod.Magnitude);
                dpParameter.Add("Offset", dtoTimePeriod.Offset);
                dpParameter.Add("PhysicalDimensionId", dtoTimePeriod.PhysicalDimensionId);
                dpParameter.Add("Id", dtoTimePeriod.Id);

                int iResult = -1;

                iResult = await sqlDataAccess.Connection.ExecuteAsync(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (iResult < 1)
                    return new RepositoryResult<bool>(new RepositoryError()
                    {
                        Code = TimePeriodError.Code.Method,
                        Description = $"Time period {dtoTimePeriod.Id} has not been updated."
                    });

                return new RepositoryResult<bool>(true);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message });
            }
        }
    }
}