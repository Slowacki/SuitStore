using MongoDB.Driver;
using SuitStore.Alterations.Core.Contracts;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Data.Services;

public class AlterationsStore(IMongoCollection<AlterationSaga> alterationsCollection) : IAlterationsStore
{
    public async Task<IEnumerable<AlterationSaga>> GetByTailorIdAsync(long tailorId, CancellationToken cancellationToken)
    {
        var alterations = await alterationsCollection.Find(a => a.TailorId == tailorId).ToListAsync(cancellationToken);

        return alterations;
    }

    public async Task<IEnumerable<AlterationSaga>> GetAsync(long? tailorId, string? state, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<AlterationSaga>.Filter;
        var filters = new List<FilterDefinition<AlterationSaga>>();

        if (tailorId is not null)
            filters.Add(filterBuilder.Eq(a => a.TailorId, tailorId));
        
        if (state is not null)
            filters.Add(filterBuilder.Eq(a => a.CurrentState, state));
        
        var filterDefinition = filters.Any() ? filterBuilder.And(filters) : FilterDefinition<AlterationSaga>.Empty;

        var alterations = await alterationsCollection.Find(filterDefinition).ToListAsync(cancellationToken);

        return alterations;
    }
}