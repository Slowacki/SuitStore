using MassTransit;
using SuitStore.Alterations.Core.Messages;
using SuitStore.Alterations.Core.Models;
using SuitStore.Email.Messaging.Commands;
using SuitStore.Email.Messaging.Models;
using SuitStore.Payments.Messaging.Events;

namespace SuitStore.Alterations.Core.Saga;

public class AlterationStateMachine : MassTransitStateMachine<AlterationSaga>
{
    public AlterationStateMachine()
    {
        ConfigureSaga();
        
        Initially(
            When(Create)
                .Then(a => a.Saga.OrderId = StartNewOrder())
                .Respond(new AlterationCreated())
                .TransitionTo(AwaitingPayment));
        
        During(AwaitingPayment,
            When(OrderPaid)
                .TransitionTo(ReadyToStart));
        
        During(ReadyToStart,
            When(StartAlteration)
                .Then(a => a.Saga.TailorId = a.Message.TailorId)
                .TransitionTo(InProgress));
        
        During(InProgress,
            When(FinishAlteration)
                .Send(a => new SendEmail(a.Saga.ClientId, EmailType.AlterationsFinished))
                .TransitionTo(Completed));
    }
    
    public State AwaitingPayment { get; set; } = null!;
    public State ReadyToStart { get; set; } = null!;
    public State InProgress { get; set; } = null!;
    public State Completed { get; set; } = null!;
    
    public Event<CreateAlteration> Create { get; set; } = null!;
    public Event<OrderPaid> OrderPaid { get; set; } = null!;
    public Event<StartAlteration> StartAlteration { get; set; } = null!;
    public Event<FinishAlteration> FinishAlteration { get; set; } = null!;
    
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
                    AlterationInstructions = message.Instructions,
                    ClientId = message.ClientId,
                    ProductId = message.ProductId
                };
            });
        });
        Event(() => OrderPaid, e =>
        {
            e.CorrelateBy((a, b) => a.OrderId == b.Message.OrderId);
            e.OnMissingInstance(configurator => configurator.Discard());
        });
        Event(() => StartAlteration, e =>
        {
            e.CorrelateBy((a, b) => a.AlterationId == b.Message.AlterationId);
            e.OnMissingInstance(configurator => configurator.ExecuteAsync(context => context.RespondAsync(new AlterationNotFound())));
        });
        Event(() => FinishAlteration, e =>
        {
            e.CorrelateBy((a, b) => a.AlterationId == b.Message.AlterationId);
            e.OnMissingInstance(configurator => configurator.ExecuteAsync(context => context.RespondAsync(new AlterationNotFound())));
        });
    }

    private long StartNewOrder()
    {
        // Assuming we have some kind of a Order/Payment client we use it here to initiate a new order to be paid and retrieve the ID to be associated with the alteration
        var rnd = new Random();
        return rnd.NextInt64(long.MaxValue);
    }
}