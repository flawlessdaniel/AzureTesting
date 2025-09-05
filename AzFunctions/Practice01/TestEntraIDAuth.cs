using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Practice01;

public class TestEntraIDAuth
{
    private readonly ILogger<TestEntraIDAuth> _logger;

    public TestEntraIDAuth(ILogger<TestEntraIDAuth> logger)
    {
        _logger = logger;
    }

    [Function("TestEntraIDAuth")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        _logger.LogInformation("Ejecutado test endpoint + Entra ID Auth");
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteStringAsync("Función ejecutada correctamente con Entra ID Auth");
        return response;
    }
}