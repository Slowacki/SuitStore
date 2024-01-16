using Microsoft.Extensions.DependencyInjection;
using SuitStore.Alterations.Core.Configuration;
using SuitStore.Alterations.Core.Contracts;
using SuitStore.Alterations.Data.Services;

namespace SuitStore.Alterations.Data.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        Action<MongoOptions> options)
    {
        var mongoOptions = new MongoOptions();
        options(mongoOptions);
        
        services.AddSingleton<IAlterationsStore, AlterationsStore>();

        return services;
    }
}