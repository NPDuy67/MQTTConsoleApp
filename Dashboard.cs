using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace MQTTSubscriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("192.168.1.153", 1883)
                            .WithCleanSession()
                            .Build();
            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected Successful");
                var toppicFilter = new TopicFilterBuilder()
                                        .WithTopic("Sensor")
                                        .Build();
                await client.SubscribeAsync(toppicFilter);
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected");
            });

            client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            await client.ConnectAsync(options);

            Console.ReadLine();

            await client.DisconnectAsync();
        }
    }
}
