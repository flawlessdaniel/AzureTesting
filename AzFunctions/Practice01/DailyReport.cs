using System;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Practice01;

public class DailyReport
{
    private readonly ILogger _logger;

    public DailyReport(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DailyReport>();
    }

    [Function("DailyReport")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        // Fake CSV data
        var csv = "id,fecha,monto\n1,2025-01-01,100.0\n2,2025-01-01,55.5\n";
        var bytes = Encoding.UTF8.GetBytes(csv);

        // Upload to Blob Storage
        var conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var container = new BlobContainerClient(conn, "reports");
        await container.CreateIfNotExistsAsync();
        var blob = container.GetBlobClient($"report-{DateTime.UtcNow:yyyyMMdd}.csv");
        using var ms = new System.IO.MemoryStream(bytes);
        await blob.UploadAsync(ms, overwrite: true);
        _logger.LogInformation("Reporte subido: {BlobName}", blob.Name);
    }
}