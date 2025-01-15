using PhysicalData.Application.Filter;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

namespace PhysicalData.Application.Interface
{
    public interface ITimePeriodRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoTimePeriod"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> DeleteAsync(TimePeriodTransferObject dtoTimePeriod, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guPassportId"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> ExistsAsync(Guid guTimePeriodId, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guId"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<TimePeriodTransferObject>> FindByIdAsync(Guid guId, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<IEnumerable<TimePeriodTransferObject>>> FindByFilterAsync(TimePeriodByFilterOption optFilter, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoTimePeriod"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> InsertAsync(TimePeriodTransferObject dtoTimePeriod, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optFilter"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<int>> QuantityByFilterAsync(TimePeriodByFilterOption optFilter, CancellationToken tknCancellation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoTimePeriod"></param>
        /// <param name="tknCancellation"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> UpdateAsync(TimePeriodTransferObject dtoTimePeriod, DateTimeOffset dtEditedAt, CancellationToken tknCancellation);
    }
}
