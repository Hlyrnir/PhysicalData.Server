using PhysicalData.Application.Filter;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

#pragma warning disable CS1998

namespace PhysicalData.Application.Test.Fake.Repository
{
    internal sealed class FakeTimePeriodRepository : ITimePeriodRepository
    {
        private readonly IDictionary<Guid, TimePeriodTransferObject> dictTimePeriod;

        public FakeTimePeriodRepository(FakeDatabase dbFake)
        {
            this.dictTimePeriod = dbFake.TimePeriod;
        }

        public async Task<RepositoryResult<bool>> DeleteAsync(TimePeriodTransferObject dtoTimePeriod, CancellationToken tknCancellation)
        {
            if (dictTimePeriod.ContainsKey(dtoTimePeriod.Id) == false)
                return new RepositoryResult<bool>(TestError.Repository.TimePeriod.NotFound);

            return new RepositoryResult<bool>(dictTimePeriod.Remove(dtoTimePeriod.Id));
        }

        public async Task<RepositoryResult<bool>> ExistsAsync(Guid guTimePeriodId, CancellationToken tknCancellation)
        {
            return new RepositoryResult<bool>(dictTimePeriod.ContainsKey(guTimePeriodId));
        }

        public async Task<RepositoryResult<TimePeriodTransferObject>> FindByIdAsync(Guid guId, CancellationToken tknCancellation)
        {
            dictTimePeriod.TryGetValue(guId, out TimePeriodTransferObject? dtoTimePeriodInRepository);

            if (dtoTimePeriodInRepository is null)
                return new RepositoryResult<TimePeriodTransferObject>(TestError.Repository.TimePeriod.NotFound);

            return new RepositoryResult<TimePeriodTransferObject>(dtoTimePeriodInRepository.Clone());
        }

        public async Task<RepositoryResult<IEnumerable<TimePeriodTransferObject>>> FindByFilterAsync(TimePeriodFilterOption optFilter, CancellationToken tknCancellation)
        {
            IList<TimePeriodTransferObject> lstTimePeriod = new List<TimePeriodTransferObject>();

            foreach (TimePeriodTransferObject dtoTimePeriod in dictTimePeriod.Values)
            {
                if (optFilter.PhysicalDimensionId is null || optFilter.PhysicalDimensionId == dtoTimePeriod.PhysicalDimensionId)
                    lstTimePeriod.Add(dtoTimePeriod);
            }

            return new RepositoryResult<IEnumerable<TimePeriodTransferObject>>(lstTimePeriod.Skip((optFilter.Page + (-1)) * optFilter.PageSize).Take(optFilter.PageSize).AsEnumerable());
        }

        public async Task<RepositoryResult<bool>> InsertAsync(TimePeriodTransferObject dtoTimePeriod, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictTimePeriod.ContainsKey(dtoTimePeriod.Id) == true)
                return new RepositoryResult<bool>(TestError.Repository.TimePeriod.Exists);

            bool bResult = dictTimePeriod.TryAdd(dtoTimePeriod.Id, dtoTimePeriod);

            return new RepositoryResult<bool>(bResult);
        }

        public async Task<RepositoryResult<int>> QuantityByFilterAsync(TimePeriodFilterOption optFilter, CancellationToken tknCancellation)
        {
            int iQuantity = dictTimePeriod.Count;

            return new RepositoryResult<int>(iQuantity);
        }

        public async Task<RepositoryResult<bool>> UpdateAsync(TimePeriodTransferObject dtoTimePeriod, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
        {
            if (dictTimePeriod.ContainsKey(dtoTimePeriod.Id) == false)
                return new RepositoryResult<bool>(TestError.Repository.TimePeriod.NotFound);

            dictTimePeriod[dtoTimePeriod.Id] = dtoTimePeriod.Clone();

            return new RepositoryResult<bool>(true);
        }
    }
}

#pragma warning restore CS1998