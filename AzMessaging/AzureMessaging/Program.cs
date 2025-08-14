using AzureMessaging.Managers;
using Microsoft.Azure.ServiceBus;

var appBuilder = WebApplication.CreateBuilder();

appBuilder.Services.AddControllers();

appBuilder.Services.AddSingleton<IQueueClient>(new QueueClient("", ""));
appBuilder.Services.AddSingleton<IServiceBusManager, ServiceBusManager>();

var app = appBuilder.Build();
app.UseRouting();
app.Run();