using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAlterationsCore(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint("alteration-saga", e =>
                {
                    e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
                    
                    e.ConfigureSaga<AlterationSaga>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}