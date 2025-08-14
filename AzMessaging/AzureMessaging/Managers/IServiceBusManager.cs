namespace AzureMessaging.Managers
{
    internal interface IServiceBusManager
    {
        Task EnqueueMessage<TMessage>(TMessage Message) where TMessage : struct;
        Task ProduceMessage<TMessage>(TMessage Message) where TMessage : struct;
    }
}