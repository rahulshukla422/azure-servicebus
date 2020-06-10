using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace azure_servicebus_reciever
{
    class Program
    {
        const string servicebusConenctionString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX;";
        const string topicName = "topic-temp";
        const string subcriptionName = "topic-sub";
        private static ISubscriptionClient subscriptionClient;
        static async Task Main()
        {
            Console.WriteLine("Receiving the message from service bus topic...");

            subscriptionClient = new SubscriptionClient(servicebusConenctionString, topicName, subcriptionName);

            // Register the message handler and recieve the messages in the loop
            RegisterMessageHander();

            Console.ReadKey();
            await subscriptionClient.CloseAsync();
        }

        private static void RegisterMessageHander()
        {
            MessageHandlerOptions messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            //Register the function to process the message
            subscriptionClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

        }

        private static async Task ProcessMessageAsync(Message msg, CancellationToken arg2)
        {
            var message = Encoding.UTF8.GetString(msg.Body);

            var response = JsonConvert.DeserializeObject<System.Collections.Generic.List<Order>>(message);

            foreach (var item in response)
            {
                Console.WriteLine($"{item.Id}  {item.Name}  {item.Amount}");
            }

            await subscriptionClient.CompleteAsync(msg.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler encountered an exception {arg.Exception}.");
            var context = arg.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }

    public class Order
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

    }
}
