using Azure.Messaging.ServiceBus;
using System.Text;

namespace ReviewService.Bus
{
    public class MessageReceiver
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;

        public MessageReceiver(string serviceBusConnectionString, string topicName, string subscriptionName)
        {
            _client = new ServiceBusClient(serviceBusConnectionString);
            _processor = _client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());
        }

        public async Task RegisterOnMessageHandlerAndReceiveMessagesAsync()
        {
            _processor.ProcessMessageAsync += ProcessMessagesAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync();
        }

        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            string body = Encoding.UTF8.GetString(args.Message.Body);
            Console.WriteLine($"Received message: {body}");

            // Complete the message. Messages are deleted from the queue or subscription.
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message handler encountered an exception {args.Exception}.");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
