using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using static csharp_mqtt_client.MqttIotIgniteDataTypes;

namespace csharp_mqtt_client
{
    class Program
    {
        // Connection settings
        private static string Username;
        private static string Password;
        private static string ClientId;

        // Some sample names 
        private static string Vendor;
        private static string Nodeid;
        private static string Topic1;
        private static string Topic2;
        private static string Topic3;


        // Replace these strings with your supplied credentials 

        static MqttIoTIgniteClient Mqtt { get; set; }
        static CancellationTokenSource CtSource { get; set; }

        static void Main(string[] args)
        {
            // Read settings
            Username = ConfigurationManager.AppSettings["Username"];
            Password = ConfigurationManager.AppSettings["Password"];
            ClientId = ConfigurationManager.AppSettings["ClientId"];
            Vendor = ConfigurationManager.AppSettings["Vendor"];
            Nodeid = ConfigurationManager.AppSettings["Nodeid"];
            Topic1 = ConfigurationManager.AppSettings["Topic1"];
            Topic2 = ConfigurationManager.AppSettings["Topic2"];
            Topic3 = ConfigurationManager.AppSettings["Topic3"];



            Console.WriteLine("C# Mqtt client");
            // Make a new client
            Mqtt = new MqttIoTIgniteClient(ClientId, Nodeid);
            // Subscribe to anny commands being sent back
            Mqtt.OnReceivedCommand += OnCommandReceived;
            // Tell the client to connect
            try
            {
                Mqtt.Connect(Username, Password).Wait();
            }
            catch (MQTTnet.Adapter.MqttConnectingFailedException ex)
            {
                Console.WriteLine("Did you change the MQTT credentials in the 'app.config'? ");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine($"ConnectionResult: {Mqtt.ConnectionStatus}");

            // Define the data output of the device (is needed only once before sending data)
            var sensors = new List<Sensor>()
            {
                new Sensor(Topic1, Sensor.DataType.STRING, "Status", Vendor),
                new Sensor(Topic2, Sensor.DataType.FLOAT, "Sensor Value", Vendor),
                new Sensor(Topic3, Sensor.DataType.INTEGER, "Actuator", Vendor, true)
            };
            Mqtt.SendInventoryMessage(sensors).Wait();

            // Send random numeric data to the client
            Task.Run(SendRandomDataAsync);

            // Send text typed into the console to the client 
            Console.WriteLine($"Press [Enter] without text to exit.");
            Console.WriteLine($"Type text and press [Enter] to send a status message to the '{Topic1}' topic.");
            string input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                Mqtt.SendStatus(Topic1, DateTimeOffset.Now, input).Wait();
                input = Console.ReadLine();
            }

            Mqtt.Disconnect().Wait();
            CtSource.Cancel();
        }

        /// <summary>
        /// Sample of a method sending numeric data
        /// </summary>
        /// <returns></returns>
        static async Task SendRandomDataAsync()
        {
            Random random = new Random();
            while (!CtSource.Token.IsCancellationRequested)
            {
                float x = 1 + 5 * random.Next();
                float y = 1 + 5 * random.Next();
                await Mqtt.SendNumericData(Topic2, DateTimeOffset.Now, new[] { x, y });
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Event Triggered when the client receives a command
        /// </summary>
        /// <param name="command"></param>
        private static void OnCommandReceived(bool command)
        {
            Console.WriteLine($"Received command {command}");
        }
    }
}
