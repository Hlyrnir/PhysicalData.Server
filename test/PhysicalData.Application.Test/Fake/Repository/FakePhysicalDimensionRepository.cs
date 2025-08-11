using PhysicalData.Application.Filter;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using PhysicalData.Application.Transfer;

#pragma warning disable CS1998

namespace PhysicalData.Application.Test.Fake.Repository
{
    internal sealed class FakePhysicalDimensionRepository : IPhysicalDimensionRepository
    {
    private readonly IDictionary<Guid, PhysicalDimensionTransferObject> dictPhysicalDimension;

        public FakePhysicalDimensionRepository(FakeDatabase dbFake)
        {
            this.dictPhysicalDimension = dbFake.PhysicalDimension;
        }

        public async Task<RepositoryResult<bool>> DeleteAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, CancellationToken tknCancellation)
        {
            if (dictPhysicalDimension.ContainsKey(dtoPhysicalDimension.Id) == false)
                return new RepositoryResult<bool>(TestError.Repository.PhysicalDimension.NotFound);

            return new RepositoryResult<bool>(dictPhysicalDimension.Remove(dtoPhysicalDimension.Id));
        }

        public async Task<RepositoryResult<bool>> ExistsAsync(Guid guPhysicalDimensionId, CancellationToken tknCancellation)
        {
            return new RepositoryResult<bool>(dictPhysicalDimension.ContainsKey(guPhysicalDimensionId));
        }

        public async Task<RepositoryResult<PhysicalDimensionTransferObject>> FindByIdAsync(Guid guId, CancellationToken tknCancellation)
        {
            dictPhysicalDimension.TryGetValue(guId, out PhysicalDimensionTransferObject? dtoPhysicalDimensionInRepository);

            if (dtoPhysicalDimensionInRepository is null)
                return new RepositoryResult<PhysicalDimensionTransferObject>(TestError.Repository.PhysicalDimension.NotFound);

            return new RepositoryResult<PhysicalDimensionTransferObject>(dtoPhysicalDimensionInRepository.Clone());
        }

        public async Task<RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>> FindByFilterAsync(PhysicalDimensionFilterOption optFilter, CancellationToken tknCancellation)
        {
            IList<PhysicalDimensionTransferObject> lstPhysicalDimension = new List<PhysicalDimensionTransferObject>();

            foreach (PhysicalDimensionTransferObject dtoPhysicalDimension in dictPhysicalDimension.Values)
            {
                if (optFilter.ConversionFactorToSI is null || optFilter.ConversionFactorToSI == dtoPhysicalDimension.ConversionFactorToSI
                    && optFilter.CultureName is null || optFilter.CultureName is not null && dtoPhysicalDimension.CultureName.Contains(optFilter.CultureName) == true
                    && optFilter.ExponentOfAmpere is null || optFilter.ExponentOfAmpere == dtoPhysicalDimension.ExponentOfAmpere
                    && optFilter.ExponentOfCandela is null || optFilter.ExponentOfCandela == dtoPhysicalDimension.ExponentOfCandela
                    && optFilter.ExponentOfKelvin is null || optFilter.ExponentOfKelvin == dtoPhysicalDimension.ExponentOfKelvin
                    && optFilter.ExponentOfKilogram is null || optFilter.ExponentOfKilogram == dtoPhysicalDimension.ExponentOfKilogram
                    && optFilter.ExponentOfMetre is null || optFilter.ExponentOfMetre == dtoPhysicalDimension.ExponentOfMetre
                    && optFilter.ExponentOfMole is null || optFilter.ExponentOfMole == dtoPhysicalDimension.ExponentOfMole
                    && optFilter.ExponentOfSecond is null || optFilter.ExponentOfSecond == dtoPhysicalDimension.ExponentOfSecond
                    && optFilter.Name is null || optFilter.Name is not null && dtoPhysicalDimension.Name.Contains(optFilter.Name) == true
                    && optFilter.Symbol is null || optFilter.Symbol is not null && dtoPhysicalDimension.Symbol.Contains(optFilter.Symbol) == true
                    && optFilter.Unit is null || optFilter.Unit is not null && dtoPhysicalDimension.Unit.Contains(optFilter.Unit) == true)
                    lstPhysicalDimension.Add(dtoPhysicalDimension);
            }

            return new RepositoryResult<IEnumerable<PhysicalDimensionTransferObject>>(lstPhysicalDimension.Skip((optFilter.Page + (-1)) * optFilter.PageSize).Take(optFilter.PageSize).AsEnumerable());
        }

        public async Task<RepositoryResult<bool>> InsertAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictPhysicalDimension.ContainsKey(dtoPhysicalDimension.Id) == true)
                return new RepositoryResult<bool>(TestError.Repository.PhysicalDimension.Exists);

            bool bResult = dictPhysicalDimension.TryAdd(dtoPhysicalDimension.Id, dtoPhysicalDimension);

            return new RepositoryResult<bool>(bResult);
        }

        public async Task<RepositoryResult<int>> QuantityByFilterAsync(PhysicalDimensionFilterOption optFilter, CancellationToken tknCancellation)
        {
            int iQuantity = dictPhysicalDimension.Count;

            return new RepositoryResult<int>(iQuantity);
        }

        public async Task<RepositoryResult<bool>> UpdateAsync(PhysicalDimensionTransferObject dtoPhysicalDimension, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
        {
            if (dictPhysicalDimension.ContainsKey(dtoPhysicalDimension.Id) == false)
                return new RepositoryResult<bool>(TestError.Repository.PhysicalDimension.NotFound);

            dictPhysicalDimension[dtoPhysicalDimension.Id] = dtoPhysicalDimension.Clone();

            return new RepositoryResult<bool>(true);
        }
    }
}

#pragma warning restore CS1998
