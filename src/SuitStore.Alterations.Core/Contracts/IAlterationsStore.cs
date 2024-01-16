using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Contracts;

public interface IAlterationsStore
{
    Task<IEnumerable<Alteration>> GetByTailorIdAsync(long tailorId, CancellationToken cancellationToken);
    Task<IEnumerable<Alteration>> GetAsync(long? tailorId, string? state, CancellationToken cancellationToken);
}