namespace SuitStore.Alterations.Api.Requests;

public record GetAlterationsRequest(long? TailorId, string? State);