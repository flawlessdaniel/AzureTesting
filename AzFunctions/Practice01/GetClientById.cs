using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Daniel.AzureTesting;

public class GetClientById
{
    private readonly ILogger<GetClientById> _logger;

    public GetClientById(ILogger<GetClientById> logger)
    {
        _logger = logger;
    }

    [Function("GetClientById")]
    public HttpResponseData Run(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = "customers/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        var body = $"{{\"id\":\"{id}\",\"name\":\"Cliente Demo\",\"email\":\"cliente.demo@example.com\",\"status\":\"Active\"}}";
        response.WriteStringAsync(body);
        return response;
    }
}