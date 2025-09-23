using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace Practice01.FuncTest;

public static class Orchestration
{
    [Function(nameof(Orchestration))]
    public static async Task<int> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var routes = new List<string> { "R1", "R2", "R3", "R4" };
        var tasks = new List<Task<int>>();
        foreach (var r in routes)
        {
            tasks.Add(context.CallActivityAsync<int>(nameof(CalcRouteActivity), r));
        }
        var results = await Task.WhenAll(tasks);
        var total = 0;
        foreach (var t in results) total += t;
        return total;
    }

    [Function(nameof(CalcRouteActivity))]
    public static async Task<int> CalcRouteActivity([ActivityTrigger] string route, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("CalcRouteActivity");
        logger.LogInformation("Calculando ruta {Route}", route);
        await Task.Delay(TimeSpan.FromMilliseconds(500)); // simular cálculo
        return new Random().Next(10, 60); // minutos estimados
    }

    [Function("Orchestration_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("Orchestration_HttpStart");

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(Orchestration));

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}