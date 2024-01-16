using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Saga;

public class AlterationSaga : SagaStateMachineInstance
{
    [BsonId]
    public Guid CorrelationId { get; set; }
    public Guid AlterationId { get; set; } = default!;
    public string? OrderId { get; set; }
    public AlterationInstructions AlterationInstructions { get; set; } = default!;
    public DateTime CreatedAtDateUtc { get; set; }
    public DateTime? CompletedAtDateUtc { get; set; }
    public string CurrentState { get; set; } = default!;
}