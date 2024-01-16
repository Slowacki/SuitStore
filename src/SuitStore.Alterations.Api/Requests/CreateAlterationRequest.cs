using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Api.Requests;

public record CreateAlterationRequest(AlterationInstructions AlterationInstructions, long ProductId);