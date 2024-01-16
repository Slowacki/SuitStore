using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Api.Requests;

public record CreateAlterationRequest(IEnumerable<AlterationInstruction> AlterationInstructions, long ProductId);