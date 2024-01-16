using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Messages;

public record CreateAlteration(long ClientId, long ProductId, IEnumerable<AlterationInstruction> Instructions);