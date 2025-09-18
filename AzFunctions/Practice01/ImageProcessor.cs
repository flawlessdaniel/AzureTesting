using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Practice01;

public class ImageProcessor
{
    private readonly ILogger<ImageProcessor> _logger;

    public ImageProcessor(ILogger<ImageProcessor> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ImageProcessor))]
    public async Task Run([BlobTrigger("incoming-images/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
    {
        _logger.LogInformation("Procesando imagen {Name}, tamaño: {Len}", name, stream?.Length ?? 0);
        var processedBytes = Encoding.UTF8.GetBytes($"PROCESSED-{DateTime.UtcNow:o}-{name}");
        var conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var container = new BlobContainerClient(conn, "processed-images");
        await container.CreateIfNotExistsAsync();
        var blob = container.GetBlobClient(name);
        using var ms = new System.IO.MemoryStream(processedBytes);
        await blob.UploadAsync(ms, overwrite: true);
        _logger.LogInformation("Imagen procesada guardada en: processed-images/{Name}", name);
    }
}