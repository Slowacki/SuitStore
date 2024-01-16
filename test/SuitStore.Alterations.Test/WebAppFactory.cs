using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SuitStore.Alterations.Api;
using SuitStore.Alterations.Core.Saga;

namespace SuitStore.Alterations.Test;

public sealed class WebAppFactory : WebApplicationFactory<Program>
{
    public IMongoCollection<AlterationSaga> AlterationsCollection { get; }

    public WebAppFactory()
    {
        AlterationsCollection = Services.GetRequiredService<IMongoCollection<AlterationSaga>>();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
    }
}