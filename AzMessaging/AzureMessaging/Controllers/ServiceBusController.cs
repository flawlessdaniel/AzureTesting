using AzureMessaging.DTOs;
using AzureMessaging.Managers;
using Microsoft.AspNetCore.Mvc;

namespace AzureMessaging.Controllers
{
    [ApiController]
    [Route("ServiceBus")]
    internal class ServiceBusController : ControllerBase
    {
        private readonly IServiceBusManager _serviceBusManager;

        public ServiceBusController(IServiceBusManager serviceBusManager)
        {
            _serviceBusManager = serviceBusManager;
        }

        [HttpGet("EnqueueSimpleHello")]
        public void EnqueueSimpleHelloMessage()
        {
            _serviceBusManager.EnqueueMessage<SimpleHello>(new SimpleHello("Hola Mundo"));
        }

        [HttpGet("ProduceSimpleHello")]
        public void ProduceSimpleHelloMessage()
        {
            _serviceBusManager.ProduceMessage<SimpleHello>(new SimpleHello("Hola Mundo"));
        }
    }
}
