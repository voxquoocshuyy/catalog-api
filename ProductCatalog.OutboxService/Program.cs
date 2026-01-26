using EventBus.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalog.Events;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.OutboxService;
using ProductCatalog.ServiceDefaults;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
var eventAssembly = typeof(ProductCreatedEvent).Assembly;

builder.AddServiceDefaults();

builder.AddKafkaProducer("kafka");
builder.AddKafkaEventPublisher("catalog-events");

builder.Services.AddSingleton(s =>
{
    var connectionString = builder.Configuration.GetConnectionString("catalogdb");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'catalogdb' not found or is empty.");
    }

    return new TransactionalOutboxLogTailingServiceOptions()
    {
        ConnectionString = connectionString,
        PayloadTypeResolver = (type) =>
        {
            var resolvedType = (eventAssembly ?? Assembly.GetExecutingAssembly()).GetType(type);
            if (resolvedType == null)
            {
                throw new InvalidOperationException($"Could not resolve event type: {type}. Ensure the type exists in the ProductCatalog.Events assembly.");
            }
            return resolvedType;
        },
    };
});

builder.AddNpgsqlDbContext<ProductCatalogDbContext>("catalogdb", configureDbContextOptions: dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(builder =>
    {
    });
}); 

builder.Services.AddHostedService<TransactionalOutboxLogTailingService>();

var host = builder.Build();
host.Run();
