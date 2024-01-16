using MassTransit;
using SuitStore.Alterations.Core.Messages;
using SuitStore.Alterations.Core.Models;
using SuitStore.Payments.Messaging.Events;

namespace SuitStore.Alterations.Core.Saga;

public class AlterationStateMachine : MassTransitStateMachine<AlterationSaga>
{
    public AlterationStateMachine()
    {
        Initially(
            When(Create)
                // Create new order
                .TransitionTo(AwaitingPayment));
        
        During(AwaitingPayment,
            When(OrderPaid)
                .TransitionTo(ReadyToStart));
        
        During(ReadyToStart,
            When(AlterationStarted)
                .TransitionTo(InProgress));
        
        During(InProgress,
            When(AlterationFinished)
                // Send Email
                .TransitionTo(Completed));
    }
    
    public State AwaitingPayment { get; set; } = null!;
    public State ReadyToStart { get; set; } = null!;
    public State InProgress { get; set; } = null!;
    public State Completed { get; set; } = null!;
    
    public Event<CreateAlteration> Create { get; set; } = null!;
    public Event<OrderPaid> OrderPaid { get; set; } = null!;
    public Event<AlterationStarted> AlterationStarted { get; set; } = null!;
    public Event<AlterationFinished> AlterationFinished { get; set; } = null!;
    
    private void ConfigureSaga()
    {
        InstanceState(x => x.CurrentState);
        SetCompletedWhenFinalized();

        State(() => AwaitingPayment);
        State(() => ReadyToStart);
        State(() => InProgress);
        State(() => Completed);
        Event(() => Create, e =>
        {
            var id = Guid.NewGuid();
            e.CorrelateById(_ => id);
            e.InsertOnInitial = true;
            e.SetSagaFactory(context =>
            {
                var message = context.Message;
                
                return new AlterationSaga
                {
                    CorrelationId = id,
                    CreatedAtDateUtc = DateTime.UtcNow,
                    AlterationId = id,
                    AlterationInstructions = new AlterationInstructions(1,1)
                };
            });
        });
        Event(() => OrderPaid, e =>
        {
            e.CorrelateBy((a, b) => a.OrderId == b.Message.OrderId);
            e.OnMissingInstance(configurator => configurator.Discard());
        });
        Event(() => AlterationStarted, e =>
        {
            e.CorrelateBy((a, b) => a.AlterationId == b.Message.AlterationId);
        });
        Event(() => AlterationFinished, e =>
        {
            e.CorrelateBy((a, b) => a.AlterationId == b.Message.AlterationId);
        });
    }
}