using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Saga;

public class AlterationSaga : SagaStateMachineInstance
{
    [BsonId]
    public Guid CorrelationId { get; set; }
    public Guid AlterationId { get; set; }
    public long? OrderId { get; set; }
    public long ClientId { get; set; }
    public long? TailorId { get; set; }
    public AlterationInstructions AlterationInstructions { get; set; } = default!;
    public DateTime CreatedAtDateUtc { get; set; }
    public DateTime? CompletedAtDateUtc { get; set; }
    public string CurrentState { get; set; } = default!;
}