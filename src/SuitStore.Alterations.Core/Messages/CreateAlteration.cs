using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Messages;

public record CreateAlteration(long ClientId, AlterationInstructions Instructions);