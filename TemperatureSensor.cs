using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;

namespace MQTTPublisher
{
    class TemperatureSensor
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("test.mosquitto.org",1883)
                            .WithCleanSession()
                            .Build();
            client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected Successful");
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected");
            });

            await client.ConnectAsync(options);

            Console.WriteLine("Press a key to public the message");
            Console.ReadLine();

            await PublishMessageAsync(client);

            await client.DisconnectAsync();

        }
        private static async Task PublishMessageAsync(IMqttClient client)
        {
            string tmpInfo = "Current temperature is 25 degrees Celsius";
            var message = new MqttApplicationMessageBuilder()
                            .WithTopic("Sensor")
                            .WithPayload(tmpInfo)
                            .WithAtLeastOnceQoS()
                            .Build();

            if (client.IsConnected)
            {
                await client.PublishAsync(message);
            }            
        }
    }
}
