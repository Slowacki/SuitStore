using Asp.Versioning;
using SuitStore.Alterations.Core.Configuration;
using SuitStore.Alterations.Data.Configuration;

namespace SuitStore.Alterations.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;
        
        // Add services to the container.
        services.AddAuthorization();
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAlterationsCore(
            mongoOptions => configuration.Bind("MongoDb", mongoOptions),
            massTransitOptions => configuration.Bind("MassTransit", massTransitOptions));
        services.AddMongoDb(options => configuration.Bind("MongoDb", options));
        
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        app.Run();
    }
}