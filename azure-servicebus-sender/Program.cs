using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace azure_servicebus
{
    class Program
    {
        const string servicebusConnectionString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx";

        const string topicName = "topic-temp";

        private static ITopicClient topicClient;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Sending message to the topic");

            topicClient = new TopicClient(servicebusConnectionString, topicName);

            await SendMessage();

            Console.WriteLine("Message has been sent!");

            Console.ReadKey();

        }

        private static async Task SendMessage()
        {
            var serializedData = JsonConvert.SerializeObject(GetSampleMessage());
            Message message = new Message(Encoding.UTF8.GetBytes(serializedData));

            await topicClient.SendAsync(message);
        }

        private static List<Order> GetSampleMessage()
        {
            var list = new List<Order>()
            {
                new Order
                {
                    Id =1,
                    Name ="Burger",
                    Amount =1000,
                },
                new Order
                {
                    Id =1,
                    Name ="Pizza",
                    Amount =1200,
                },
                new Order
                {
                    Id =1,
                    Name ="Sandwich",
                    Amount =1100,
                }
            };

            return list;
        }
    }

    public class Order
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

    }
}
