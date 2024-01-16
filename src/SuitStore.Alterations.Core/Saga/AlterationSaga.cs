using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using SuitStore.Alterations.Core.Models;

namespace SuitStore.Alterations.Core.Saga;

public class AlterationSaga : Alteration, SagaStateMachineInstance
{
    [BsonId]
    public Guid CorrelationId { get; set; }
}