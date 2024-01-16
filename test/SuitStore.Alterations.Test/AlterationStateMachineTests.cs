using AutoFixture;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SuitStore.Alterations.Core.Saga;
using SuitStore.Payments.Messaging.Events;

namespace SuitStore.Alterations.Test;

[Collection(ComponentTestsCollection.Name)]
public class AlterationStateMachineTests
{
    private readonly IMongoCollection<AlterationSaga> _alterationsCollection;
    private readonly Fixture _fixture = new();
    private readonly IBus _bus;

    public AlterationStateMachineTests(WebAppFactory webAppFactory)
    {
        _alterationsCollection = webAppFactory.AlterationsCollection;
        _bus = webAppFactory.Services.GetRequiredService<IBus>();
    }
    
    [Fact(DisplayName = "OrderPaid event transitions the state machine to ReadyToStart state")]
    public async Task Execute_CreatesNewSaga_WhenSubmitIsSuccessful()
    {
        var orderId = _fixture.Create<long>();
        var alterationId = _fixture.Create<Guid>();
        var alterationToBeInserted = _fixture.Build<AlterationSaga>()
            .With(a => a.CurrentState, nameof(AlterationStateMachine.AwaitingPayment))
            .With(a => a.OrderId, orderId)
            .With(a => a.AlterationId, alterationId).Create();

        await _alterationsCollection.InsertOneAsync(alterationToBeInserted);

        await _bus.Publish(new OrderPaid(orderId));
        
        // TODO: Use observers instead
        await Task.Delay(TimeSpan.FromSeconds(1));

        var alteration = await _alterationsCollection.Find(a => a.AlterationId == alterationId).SingleAsync();
        
        Assert.Equal(alterationId, alteration.AlterationId);
        Assert.Equal(orderId, alteration.OrderId);
        Assert.Equal(nameof(AlterationStateMachine.ReadyToStart), alteration.CurrentState);
    }
}