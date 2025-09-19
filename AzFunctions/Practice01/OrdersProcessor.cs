using System;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Practice01;

public class OrdersProcessor
{
    public class Order
    {
        public string OrderId { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public double Amount { get; set; }
    }
    public class OrderEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Order";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string OrderId { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public double Amount { get; set; }
    }

    private readonly ILogger<OrdersProcessor> _logger;

    public OrdersProcessor(ILogger<OrdersProcessor> logger)
    {
        _logger = logger;
    }

    [Function(nameof(OrdersProcessor))]
    public async Task Run(
        [QueueTrigger("incoming-orders-queue", Connection = "AzureWebJobsStorage")]
        QueueMessage message)
    {
        _logger.LogInformation("Mensaje recibido: {Message}", message.Body);
        var order = JsonSerializer.Deserialize<Order>(message.Body)!;
        var conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var client = new TableClient(conn, "ordersresult");
        await client.CreateIfNotExistsAsync();

        var entity = new OrderEntity
        {
            PartitionKey = "Order",
            RowKey = order.OrderId ?? Guid.NewGuid().ToString(),
            OrderId = order?.OrderId ?? string.Empty,
            CustomerId = order?.CustomerId ?? string.Empty,
            Amount = order?.Amount ?? 0d
        };
        await client.AddEntityAsync(entity);
        _logger.LogInformation($"Orden {entity.OrderId} guardada en Table Storage.");
    }
}