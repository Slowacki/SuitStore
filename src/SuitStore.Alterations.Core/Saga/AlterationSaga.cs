using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Saga;

public class AlterationSaga : Alteration, SagaStateMachineInstance, ISagaVersion
{
    [BsonId]
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
}