using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Core.Contracts;

public interface IAlterationsStore
{
    Task<IEnumerable<AlterationSaga>> GetByTailorIdAsync(long tailorId, CancellationToken cancellationToken);
    Task<IEnumerable<AlterationSaga>> GetAsync(long? tailorId, string? state, CancellationToken cancellationToken);
}