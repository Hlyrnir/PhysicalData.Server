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
    internal sealed class PhysicalDimensionRepository : IPhysicalDimensionRepository
    {
        private readonly IDataAccess sqlDataAccess;

        public PhysicalDimensionRepository([FromKeyedServices(DefaultKeyedServiceName.DataAccess)] IDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;

            SqlMapper.AddTypeHandler(new GuidTypeHandler());
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> DeleteAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"DELETE FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{PhysicalDimensionColumn.Id}] = @Id";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ActualStamp", dtoPhysicalDimension.ConcurrencyStamp);
                dpParameter.Add("Id", dtoPhysicalDimension.Id);

                int iResult = -1;

                iResult = await sqlDataAccess.Connection.ExecuteAsync(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (iResult < 1)
                    return new RepositoryResult<bool>(new RepositoryError()
                    {
                        Code = PhysicalDimensionError.Code.Method,
                        Description = $"Physical dimension {dtoPhysicalDimension.Id} has not been deleted."
                    });

                return new RepositoryResult<bool>(true);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> ExistsAsync(
            Guid guPhysicalDimensionId,
            CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("Id", guPhysicalDimensionId);

                bool bResult = await sqlDataAccess.Connection.ExecuteScalarAsync<bool>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                return new RepositoryResult<bool>(bResult);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>> FindByFilterAsync(PhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT 
									[{PhysicalDimensionColumn.ExponentOfAmpere}], 
									[{PhysicalDimensionColumn.ExponentOfCandela}], 
									[{PhysicalDimensionColumn.ExponentOfKelvin}], 
									[{PhysicalDimensionColumn.ExponentOfKilogram}], 
									[{PhysicalDimensionColumn.ExponentOfMetre}], 
									[{PhysicalDimensionColumn.ExponentOfMole}], 
									[{PhysicalDimensionColumn.ExponentOfSecond}], 
									[{PhysicalDimensionColumn.ConcurrencyStamp}], 
									[{PhysicalDimensionColumn.ConversionFactorToSI}], 
									[{PhysicalDimensionColumn.CultureName}], 
									[{PhysicalDimensionColumn.Id}], 
									[{PhysicalDimensionColumn.Name}], 
									[{PhysicalDimensionColumn.Symbol}], 
									[{PhysicalDimensionColumn.Unit}] 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE (@ConversionFactorToSI IS NULL OR [{PhysicalDimensionColumn.ConversionFactorToSI}] = @ConversionFactorToSI) 
									AND (@CultureName IS NULL OR [{PhysicalDimensionColumn.CultureName}] LIKE('%'|| @CultureName ||'%')) 
									AND (@ExponentOfAmpere IS NULL OR [{PhysicalDimensionColumn.ExponentOfAmpere}] = @ExponentOfAmpere) 
									AND (@ExponentOfCandela IS NULL OR [{PhysicalDimensionColumn.ExponentOfCandela}] = @ExponentOfCandela) 
									AND (@ExponentOfKelvin IS NULL OR [{PhysicalDimensionColumn.ExponentOfKelvin}] = @ExponentOfKelvin) 
									AND (@ExponentOfKilogram IS NULL OR [{PhysicalDimensionColumn.ExponentOfKilogram}] = @ExponentOfKilogram) 
									AND (@ExponentOfMetre IS NULL OR [{PhysicalDimensionColumn.ExponentOfMetre}] = @ExponentOfMetre) 
									AND (@ExponentOfMole IS NULL OR [{PhysicalDimensionColumn.ExponentOfMole}] = @ExponentOfMole) 
									AND (@ExponentOfSecond IS NULL OR [{PhysicalDimensionColumn.ExponentOfSecond}] = @ExponentOfSecond) 
									AND (@Name IS NULL OR [{PhysicalDimensionColumn.Name}] LIKE('%'|| @Name ||'%')) 
									AND (@Symbol IS NULL OR [{PhysicalDimensionColumn.Symbol}] LIKE('%'|| @Symbol ||'%')) 
									AND (@Unit IS NULL OR [{PhysicalDimensionColumn.Unit}] LIKE('%'|| @Unit ||'%')) 
									LIMIT @PageSize 
									OFFSET @PageOffset;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ConversionFactorToSI", optFilter.ConversionFactorToSI);
                dpParameter.Add("CultureName", optFilter.CultureName);
                dpParameter.Add("ExponentOfAmpere", optFilter.ExponentOfAmpere);
                dpParameter.Add("ExponentOfCandela", optFilter.ExponentOfCandela);
                dpParameter.Add("ExponentOfKelvin", optFilter.ExponentOfKelvin);
                dpParameter.Add("ExponentOfKilogram", optFilter.ExponentOfKilogram);
                dpParameter.Add("ExponentOfMetre", optFilter.ExponentOfMetre);
                dpParameter.Add("ExponentOfMole", optFilter.ExponentOfMole);
                dpParameter.Add("ExponentOfSecond", optFilter.ExponentOfSecond);
                dpParameter.Add("Name", optFilter.Name);
                dpParameter.Add("Symbol", optFilter.Symbol);
                dpParameter.Add("Unit", optFilter.Unit);
                dpParameter.Add("PageOffset", (optFilter.Page + -1) * optFilter.PageSize);
                dpParameter.Add("PageSize", optFilter.PageSize);

                IEnumerable<PhysicalDimensionTransferObject> enumPhysicalDimensionTransferObject = await sqlDataAccess.Connection.QueryAsync<PhysicalDimensionTransferObject>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                return new RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>(enumPhysicalDimensionTransferObject);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<PhysicalDimensionTransferObject>> FindByIdAsync(Guid guPhysicalDimensionId, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<PhysicalDimensionTransferObject>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT 
									[{PhysicalDimensionColumn.ExponentOfAmpere}], 
									[{PhysicalDimensionColumn.ExponentOfCandela}], 
									[{PhysicalDimensionColumn.ExponentOfKelvin}], 
									[{PhysicalDimensionColumn.ExponentOfKilogram}], 
									[{PhysicalDimensionColumn.ExponentOfMetre}], 
									[{PhysicalDimensionColumn.ExponentOfMole}], 
									[{PhysicalDimensionColumn.ExponentOfSecond}], 
									[{PhysicalDimensionColumn.ConcurrencyStamp}], 
									[{PhysicalDimensionColumn.ConversionFactorToSI}], 
									[{PhysicalDimensionColumn.CultureName}], 
									[{PhysicalDimensionColumn.Id}], 
									[{PhysicalDimensionColumn.Name}], 
									[{PhysicalDimensionColumn.Symbol}], 
									[{PhysicalDimensionColumn.Unit}] 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.Id}] = @Id;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("Id", guPhysicalDimensionId);

                IEnumerable<PhysicalDimensionTransferObject> enumPhysicalDimensionTransferObject = await sqlDataAccess.Connection.QueryAsync<PhysicalDimensionTransferObject>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (enumPhysicalDimensionTransferObject.Count() == 0)
                    return new RepositoryResult<PhysicalDimensionTransferObject>(new RepositoryError()
                    {
                        Code = PhysicalDimensionError.Code.Method,
                        Description = $"Physical dimension {guPhysicalDimensionId} has not been found."
                    });

                return new RepositoryResult<PhysicalDimensionTransferObject>(enumPhysicalDimensionTransferObject.First());
            }
            catch (Exception exException)
            {
                return new RepositoryResult<PhysicalDimensionTransferObject>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> InsertAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"INSERT INTO [{PhysicalDimensionTable.PhysicalDimension}] (
									[{PhysicalDimensionColumn.ConcurrencyStamp}], 
									[{PhysicalDimensionColumn.ConversionFactorToSI}], 
									[{PhysicalDimensionColumn.CultureName}], 
									[{PhysicalDimensionColumn.ExponentOfAmpere}], 
									[{PhysicalDimensionColumn.ExponentOfCandela}], 
									[{PhysicalDimensionColumn.ExponentOfKelvin}], 
									[{PhysicalDimensionColumn.ExponentOfKilogram}], 
									[{PhysicalDimensionColumn.ExponentOfMetre}], 
									[{PhysicalDimensionColumn.ExponentOfMole}], 
									[{PhysicalDimensionColumn.ExponentOfSecond}], 
									[{PhysicalDimensionColumn.Id}], 
									[{PhysicalDimensionColumn.Name}], 
									[{PhysicalDimensionColumn.Symbol}], 
									[{PhysicalDimensionColumn.Unit}]) 
									SELECT 
									@ConcurrencyStamp, 
									@ConversionFactorToSI, 
									@CultureName, 
									@ExponentOfAmpere, 
									@ExponentOfCandela, 
									@ExponentOfKelvin, 
									@ExponentOfKilogram, 
									@ExponentOfMetre, 
									@ExponentOfMole, 
									@ExponentOfSecond, 
									@Id, 
									@Name, 
									@Symbol, 
									@Unit 
									WHERE NOT EXISTS(
									SELECT 1 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.Id}] = @Id);";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ConcurrencyStamp", dtoPhysicalDimension.ConcurrencyStamp);
                dpParameter.Add("ConversionFactorToSI", dtoPhysicalDimension.ConversionFactorToSI);
                dpParameter.Add("CultureName", dtoPhysicalDimension.CultureName);
                dpParameter.Add("ExponentOfAmpere", dtoPhysicalDimension.ExponentOfAmpere);
                dpParameter.Add("ExponentOfCandela", dtoPhysicalDimension.ExponentOfCandela);
                dpParameter.Add("ExponentOfKelvin", dtoPhysicalDimension.ExponentOfKelvin);
                dpParameter.Add("ExponentOfKilogram", dtoPhysicalDimension.ExponentOfKilogram);
                dpParameter.Add("ExponentOfMetre", dtoPhysicalDimension.ExponentOfMetre);
                dpParameter.Add("ExponentOfMole", dtoPhysicalDimension.ExponentOfMole);
                dpParameter.Add("ExponentOfSecond", dtoPhysicalDimension.ExponentOfSecond);
                dpParameter.Add("Id", dtoPhysicalDimension.Id);
                dpParameter.Add("Name", dtoPhysicalDimension.Name);
                dpParameter.Add("Symbol", dtoPhysicalDimension.Symbol);
                dpParameter.Add("Unit", dtoPhysicalDimension.Unit);

                int iResult = -1;

                iResult = await sqlDataAccess.Connection.ExecuteAsync(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (iResult < 1)
                    return new RepositoryResult<bool>(new RepositoryError()
                    {
                        Code = PhysicalDimensionError.Code.Method,
                        Description = $"Physical dimension '{dtoPhysicalDimension.Name}' has not been created."
                    });

                return new RepositoryResult<bool>(true);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<int>> QuantityByFilterAsync(PhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<int>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"SELECT 
									COUNT ([{PhysicalDimensionColumn.Id}]) 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE (@ConversionFactorToSI IS NULL OR [{PhysicalDimensionColumn.ConversionFactorToSI}] = @ConversionFactorToSI) 
									AND (@CultureName IS NULL OR [{PhysicalDimensionColumn.CultureName}] LIKE('%'|| @CultureName ||'%')) 
									AND (@ExponentOfAmpere IS NULL OR [{PhysicalDimensionColumn.ExponentOfAmpere}] = @ExponentOfAmpere) 
									AND (@ExponentOfCandela IS NULL OR [{PhysicalDimensionColumn.ExponentOfCandela}] = @ExponentOfCandela) 
									AND (@ExponentOfKelvin IS NULL OR [{PhysicalDimensionColumn.ExponentOfKelvin}] = @ExponentOfKelvin) 
									AND (@ExponentOfKilogram IS NULL OR [{PhysicalDimensionColumn.ExponentOfKilogram}] = @ExponentOfKilogram) 
									AND (@ExponentOfMetre IS NULL OR [{PhysicalDimensionColumn.ExponentOfMetre}] = @ExponentOfMetre) 
									AND (@ExponentOfMole IS NULL OR [{PhysicalDimensionColumn.ExponentOfMole}] = @ExponentOfMole) 
									AND (@ExponentOfSecond IS NULL OR [{PhysicalDimensionColumn.ExponentOfSecond}] = @ExponentOfSecond) 
									AND (@Name IS NULL OR [{PhysicalDimensionColumn.Name}] LIKE('%'|| @Name ||'%')) 
									AND (@Symbol IS NULL OR [{PhysicalDimensionColumn.Symbol}] LIKE('%'|| @Symbol ||'%')) 
									AND (@Unit IS NULL OR [{PhysicalDimensionColumn.Unit}] LIKE('%'|| @Unit ||'%'));";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ConversionFactorToSI", optFilter.ConversionFactorToSI);
                dpParameter.Add("CultureName", optFilter.CultureName);
                dpParameter.Add("ExponentOfAmpere", optFilter.ExponentOfAmpere);
                dpParameter.Add("ExponentOfCandela", optFilter.ExponentOfCandela);
                dpParameter.Add("ExponentOfKelvin", optFilter.ExponentOfKelvin);
                dpParameter.Add("ExponentOfKilogram", optFilter.ExponentOfKilogram);
                dpParameter.Add("ExponentOfMetre", optFilter.ExponentOfMetre);
                dpParameter.Add("ExponentOfMole", optFilter.ExponentOfMole);
                dpParameter.Add("ExponentOfSecond", optFilter.ExponentOfSecond);
                dpParameter.Add("Name", optFilter.Name);
                dpParameter.Add("Symbol", optFilter.Symbol);
                dpParameter.Add("Unit", optFilter.Unit);

                int iQuantity = 0;

                iQuantity = await sqlDataAccess.Connection.QuerySingleAsync<int>(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                return new RepositoryResult<int>(iQuantity);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<int>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<RepositoryResult<bool>> UpdateAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted);

            try
            {
                string sStatement = @$"UPDATE [{PhysicalDimensionTable.PhysicalDimension}] SET 
									[{PhysicalDimensionColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{PhysicalDimensionColumn.ConversionFactorToSI}] = @ConversionFactorToSI, 
									[{PhysicalDimensionColumn.CultureName}] = @CultureName, 
									[{PhysicalDimensionColumn.ExponentOfAmpere}] = @ExponentOfAmpere,
									[{PhysicalDimensionColumn.ExponentOfCandela}] = @ExponentOfCandela, 
									[{PhysicalDimensionColumn.ExponentOfKelvin}] = @ExponentOfKelvin,
									[{PhysicalDimensionColumn.ExponentOfKilogram}] = @ExponentOfKilogram, 
									[{PhysicalDimensionColumn.ExponentOfMetre}] = @ExponentOfMetre, 
									[{PhysicalDimensionColumn.ExponentOfMole}] = @ExponentOfMole, 
									[{PhysicalDimensionColumn.ExponentOfSecond}] = @ExponentOfSecond, 
									[{PhysicalDimensionColumn.Name}] = @Name, 
									[{PhysicalDimensionColumn.Symbol}] = @Symbol, 
									[{PhysicalDimensionColumn.Unit}] = @Unit 
									WHERE [{PhysicalDimensionColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{PhysicalDimensionColumn.Id}] = @Id;";

                DynamicParameters dpParameter = new DynamicParameters();
                dpParameter.Add("ActualStamp", dtoPhysicalDimension.ConcurrencyStamp);
                dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
                dpParameter.Add("ConversionFactorToSI", dtoPhysicalDimension.ConversionFactorToSI);
                dpParameter.Add("CultureName", dtoPhysicalDimension.CultureName);
                dpParameter.Add("ExponentOfAmpere", dtoPhysicalDimension.ExponentOfAmpere);
                dpParameter.Add("ExponentOfCandela", dtoPhysicalDimension.ExponentOfCandela);
                dpParameter.Add("ExponentOfKelvin", dtoPhysicalDimension.ExponentOfKelvin);
                dpParameter.Add("ExponentOfKilogram", dtoPhysicalDimension.ExponentOfKilogram);
                dpParameter.Add("ExponentOfMetre", dtoPhysicalDimension.ExponentOfMetre);
                dpParameter.Add("ExponentOfMole", dtoPhysicalDimension.ExponentOfMole);
                dpParameter.Add("ExponentOfSecond", dtoPhysicalDimension.ExponentOfSecond);
                dpParameter.Add("Id", dtoPhysicalDimension.Id);
                dpParameter.Add("Name", dtoPhysicalDimension.Name);
                dpParameter.Add("Symbol", dtoPhysicalDimension.Symbol);
                dpParameter.Add("Unit", dtoPhysicalDimension.Unit);

                int iResult = -1;

                iResult = await sqlDataAccess.Connection.ExecuteAsync(
                    sql: sStatement,
                    param: dpParameter,
                    transaction: sqlDataAccess.Transaction);

                if (iResult < 1)
                    return new RepositoryResult<bool>(new RepositoryError()
                    {
                        Code = PhysicalDimensionError.Code.Method,
                        Description = $"Physical dimension {dtoPhysicalDimension.Id} has not been updated."
                    });

                return new RepositoryResult<bool>(true);
            }
            catch (Exception exException)
            {
                return new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message });
            }
        }
    }
}