using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using static csharp_mqtt_client.MqttIotIgniteDataTypes;

namespace csharp_mqtt_client
{
    class MqttIoTIgniteClient
    {
        /// <summary>
        /// ClientId Property
        /// </summary>
        static string ClientId { get; set; }
        /// <summary>
        /// Node Property
        /// </summary>
        static string Node { get; set; }
        /// <summary>
        /// Client Property
        /// </summary>
        public static IMqttClient mqttClient;
        /// <summary>
        /// Cancelation token source to stop tasks on other threads
        /// </summary>
        private CancellationTokenSource ctSource = new CancellationTokenSource();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MqttIoTIgniteClient(string clientId, string node)
        {
            ClientId = clientId;
            Node = node;
        }

        /// <summary>
        /// Connection status property of the client
        /// </summary>
        public MqttClientConnectResultCode ConnectionStatus { get; private set; }

        /// <summary>
        /// Call this method to (re)connect
        /// </summary>
        public async Task Connect(string username, string password, string brokerUrl = "mqtt.ardich.com", int serverport = 8883)
        {
            ctSource = new CancellationTokenSource();
            // Setup and start a managed MQTT client.
            var options = new MqttClientOptionsBuilder()
                    .WithClientId(ClientId)
                    .WithTcpServer(brokerUrl, serverport)
                    .WithCredentials(username, password)
                    .WithTls()
                    .Build();
            mqttClient = new MqttFactory().CreateMqttClient();

            ConnectionStatus = (await mqttClient.ConnectAsync(options, ctSource.Token)).ResultCode;
            if(ConnectionStatus == MqttClientConnectResultCode.Success)
            {
                Listen();
            }
        }

        /// <summary>
        /// Call this method to disconnect the client
        /// </summary>
        public async Task Disconnect()
        {
            await mqttClient.DisconnectAsync();
            ctSource.Cancel();
            ConnectionStatus = MqttClientConnectResultCode.UnspecifiedError;
        }

        /// <summary>
        /// Send Inventory message. Required at least once before sending data
        /// </summary>
        /// <returns></returns>
        public async Task SendInventoryMessage(IEnumerable<Sensor> sensors)
        {

            var payload = new Inventory(Node, sensors).ToJson();

            // Publish DeviceNodeInventory
            var inventoryMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"{ClientId}/publish/DeviceProfile/Status/DeviceNodeInventory")
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(inventoryMessage);


            // Publish DeviceNodePresence
            payload = new PresenceData(sensors.Select(s => new InventoryItem(Node, s.Id, null, 1))).ToJson();
            var presenceMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"{ClientId}/publish/DeviceProfile/Status/DeviceNodePresence")
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            await mqttClient.PublishAsync(presenceMessage);
        }

        /// <summary>
        /// Sample of how to send a single string data
        /// </summary>
        public async Task SendStatus(string topic, DateTimeOffset time, string status)
        {
            var sensorData = new SensorDataValue<string>[] { new SensorDataValue<string>(time, new string[] { status }) };
            var payload = new SensorDataPackage<string>(sensorData).ToJson();

            // Publish DeviceNodeInventory
            var roomMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"{ClientId}/publish/DeviceProfile/{Node}/{topic}")
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(roomMessage);
        }


        /// <summary>
        /// Sample of how to send multiple numeric data
        /// </summary>
        public async Task SendNumericData(string topic, DateTimeOffset time, IEnumerable<float> measurement)
        {
            var sensorData = new[] { new SensorDataValue<float>(time, measurement) };
            var payload = new SensorDataPackage<float>(sensorData).ToJson();

            // Publish DeviceNodeInventory
            var roomMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"{ClientId}/publish/DeviceProfile/{Node}/{topic}")
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(roomMessage);
        }


        /// <summary>
        /// Listen to messages received
        /// </summary>
        public void Listen()
        {
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (payload.Contains("message\":\"On\""))
                {
                    OnReceivedCommand?.Invoke(true);
                }
                else if (payload.Contains("message\":\"Off\""))
                {
                    OnReceivedCommand?.Invoke(false);
                }
                else
                {
                    OnReceivedMessage?.Invoke(this, new MqttIoTIgniteReceivedMessageEventArgs(e));
                }
            });
        }

        // Declare the delegate (if using non-generic pattern).
        public delegate void OnReceivedMessageEventHandler(object sender, MqttIoTIgniteReceivedMessageEventArgs e);

        // Declare the event.
        public event OnReceivedMessageEventHandler OnReceivedMessage;

        // Declare the delegate (if using non-generic pattern).
        public delegate void OnReceivedCommandEventHandler(bool e);

        // Declare the event.
        public event OnReceivedCommandEventHandler OnReceivedCommand;
    }
}