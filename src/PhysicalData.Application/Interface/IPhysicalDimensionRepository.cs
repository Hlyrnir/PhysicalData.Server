using PhysicalData.Application.Filter;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Interface
{
    public interface IPhysicalDimensionRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoPhysicalDimension"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guPhysicalDimensionId"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> ExistsAsync(Guid guPhysicalDimensionId, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guId"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<PhysicalDimensionTransferObject>> FindByIdAsync(Guid guId, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>> FindByFilterAsync(PhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoPhysicalDimension"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> InsertAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optFilter"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<int>> QuantityByFilterAsync(PhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoPhysicalDimension"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> UpdateAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, DateTimeOffset dtEditedAt, CancellationToken tknCancellation);
    }
}
