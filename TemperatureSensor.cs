using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace MQTTPublisher
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
            client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected Successful");
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected");
            });

            await client.ConnectAsync(options);

            Console.WriteLine("Press a key to public the infomation of temperature sensor");
            Console.ReadLine();

            await PublishObjectAsync(client);

            await client.DisconnectAsync();

        }
        private static async Task PublishObjectAsync(IMqttClient client)
        {
            string json = JsonSerializer.Serialize(new TemperatureSensor().GetProducts());
            var tmpInfo = new MqttApplicationMessageBuilder()
                            .WithTopic("Sensor")
                            .WithPayload(json)
                            .WithAtLeastOnceQoS()
                            .Build();

            if (client.IsConnected)
            {
                await client.PublishAsync(tmpInfo);
            }            
        }
    }
    public class TemperatureSensor
    {
        public string Name { get; set; }
        public string Machine { get; set; }
        public int Value { get; set; }
        public ObservableCollection<TemperatureSensor> GetProducts()
        {
            return new ObservableCollection<TemperatureSensor>()
            {
                new TemperatureSensor(){Name = "DS18B20", Machine = "May ep nhua", Value = 52},
                new TemperatureSensor(){Name = "Omron E52-CA1DY M6 2M", Machine = "May ep cao su", Value = 60},
                new TemperatureSensor(){Name = "IFM TA2542 ", Machine = "May ep Lagging", Value = 57},
            };
        }
    }
}
