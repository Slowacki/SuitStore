using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Core.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAlterationsCore(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.AddSagaStateMachine<AlterationStateMachine, AlterationSaga>()
                .MongoDbRepository(r =>
                {
                    r.Connection = "mongodb://localhost:27017";
                    r.DatabaseName = "alterations";
                    r.CollectionName = "alteration";
                });
            
            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                // cfg.ReceiveEndpoint("alteration-saga", e =>
                // {
                //     e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
                //     
                //     e.configures
                // });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}