using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Core.Configuration;

public static class ServiceCollectionExtensions
{
    private const string AlterationSagaCollectionName = "alteration";
    
    public static IServiceCollection AddAlterationsCore(this IServiceCollection services,
        Action<MongoOptions> mongoOptionsAction,
        Action<MassTransitOptions> massTransitOptionsAction)
    {
        var mongoOptions = new MongoOptions();
        mongoOptionsAction(mongoOptions);
        
        var massTransitOptions = new MassTransitOptions();
        massTransitOptionsAction(massTransitOptions);
        
        services.AddMassTransit(bus =>
        {
            bus.AddSagaStateMachine<AlterationStateMachine, AlterationSaga>()
                .MongoDbRepository(r =>
                {
                    r.Connection = mongoOptions.ConnectionString;
                    r.DatabaseName = mongoOptions.DatabaseName;
                    r.CollectionName = AlterationSagaCollectionName;
                });
            
            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(massTransitOptions.Url);

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}