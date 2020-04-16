using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace csharp_mqtt_client
{
    class MqttIotIgniteDataTypes
    {

        /// <summary>
        /// Each client should provide its sensor inventory at least once to the IoT Ignite Platform. Providing inventory enables the platform to display sensor data in the dashboard.
        /// </summary>
        public class Inventory
        {
            /// <summary>
            /// Default constructor
            /// <summary>
            public Inventory(string nodeId, IEnumerable<Sensor> things)
            {
                Nodes = new List<Node> { new Node(nodeId, things) };
            }

            [JsonProperty(PropertyName = "data")]
            public List<Node> Nodes { get; set; } = new List<Node>();

            /// <summary>
            /// Serialize to JSON
            /// </summary>
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        /// <summary>
        /// Inventory item template
        /// </summary>
        public class InventoryItem
        {

            [JsonProperty(PropertyName = "nodeId")]
            public string NodeId { get; set; }
            [JsonProperty(PropertyName = "sensorId")]
            public string SensorId { get; set; }
            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }
            [JsonProperty(PropertyName = "connected")]
            public int Connected { get; set; }

            public InventoryItem(string nodeId, string sensorId, string description, int connected)
            {
                NodeId = nodeId;
                SensorId = sensorId;
                Description = description;
                Connected = connected;
            }
        }


        public class PresenceData
        {
            public PresenceData(IEnumerable<InventoryItem> data)
            {
                Data.AddRange(data);
            }

            [JsonProperty(PropertyName = "data")]
            List<InventoryItem> Data { get; set; } = new List<InventoryItem>();
            /// <summary>
            /// Serialize to JSON
            /// </summary>
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public class Node
        {
            /// <summary>
            /// Default constructor
            /// <summary>
            public Node(string nodeId, IEnumerable<Sensor> sensors)
            {
                NodeId = nodeId;
                Sensors.AddRange(sensors);
            }
            [JsonProperty(PropertyName = "nodeId")]
            public string NodeId;
            /// <summary>
            /// “things” array includes all sensors and actuators of a node. Sensors are physical or virtual data collectors and actuators are things that can do operations according to input from platform or client itself.
            /// </summary>
            [JsonProperty(PropertyName = "things")]
            List<Sensor> Sensors { get; set; } = new List<Sensor>(); 
        }

        public class Sensor
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="id">“id” of a sensor is unique for both node and client. It can be anything meaningful for users who will use sensor value.</param>
            /// <param name="dataType">“dataType” indicates the data type of the sensor’s generated data.</param>
            /// <param name="type">“type” is sensors or actuators common type. (Ex: Temperature Sensor, GPS, etc.)</param>
            /// <param name="vendor">“vendor” is an informational label for users. It can be the name of the manufacturer of a sensor.</param>
            /// <param name="isActuator">“actuator” is a boolean value that indicates if this thing is a sensor or actuator. For sensors it is false.</param>

            public Sensor(string id, DataType dataType, string type, string vendor, bool isActuator = false)
            {
                Id = id;
                Datatype = dataType.ToString();
                Type = type;
                Vendor = vendor;
                IsActuator = isActuator;
            }
            /// <summary>
            /// “id” of a sensor is unique for both node and client. It can be anything meaningful for users who will use sensor value.
            /// </summary>
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            /// <summary>
            /// “dataType” indicates the data type of the sensor’s generated data. {INTEGER|FLOAT|STRING}
            /// </summary>
            [JsonProperty(PropertyName = "dataType")]
            public string Datatype { get; set; }
            /// <summary>
            /// “vendor” is an informational label for users. It can be the name of the manufacturer of a sensor.
            /// </summary>
            [JsonProperty(PropertyName = "vendor")]
            public string Vendor { get; set; }
            /// <summary>
            /// “actuator” is a boolean value that indicates if this thing is a sensor or actuator. For sensors it is false.{true|false}
            /// </summary>
            [JsonProperty(PropertyName = "actuator")]
            public bool IsActuator { get; set; } = false;
            /// <summary>
            /// “type” is sensors or actuators common type. (Ex: Temperature Sensor, GPS, etc.)
            /// </summary>
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }
            /// <summary>
            /// DataTypes
            /// </summary>
            public enum DataType
            {
                INTEGER,
                FLOAT,
                STRING
            }
        }

        public class SensorDataValue<T>
        {
            public SensorDataValue(DateTimeOffset time, IEnumerable<T> values)
            {
                Date = time.ToUnixTimeMilliseconds();
                Values.AddRange(values);
            }

            [JsonProperty(PropertyName = "date")]
            public long Date { get; set; }
            [JsonProperty(PropertyName = "values")]
            private List<T> Values { get; set; } = new List<T>();

            public SensorDataValue(long time, IEnumerable<T> values)
            {
                Date = time;
                Values.AddRange(values);
            }
        }

        public class SensorDataPackage<T>
        {
            public SensorDataPackage(IEnumerable<SensorDataValue<T>> data)
            {
                Data.SensorData.AddRange(data);
            }
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }

            [JsonProperty(PropertyName = "data")]
            private SensorDataPackageData<T> Data { get; set; } = new SensorDataPackageData<T>();

            public class SensorDataPackageData<TT>
            {
                public SensorDataPackageData()
                {
                }

                [JsonProperty(PropertyName = "sensorData")]
                public List<SensorDataValue<TT>> SensorData { get; set; } = new List<SensorDataValue<TT>>();
            }

        }

    }
}
