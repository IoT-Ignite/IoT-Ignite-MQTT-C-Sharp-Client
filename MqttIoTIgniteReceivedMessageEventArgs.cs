using System.Text;
using MQTTnet;

namespace csharp_mqtt_client
{
    /// <summary>
    /// Typed EventArgs for Receiving messages
    /// </summary>
    public class MqttIoTIgniteReceivedMessageEventArgs
    {
        public MqttIoTIgniteReceivedMessageEventArgs(MqttApplicationMessageReceivedEventArgs e)
        {
            Topic = e.ApplicationMessage.Topic;
            Payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            QoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
            Retain = e.ApplicationMessage.Retain;
        }

        public string Topic { get; private set; } 
        public string Payload { get; private set; } 
        public string QoS { get; private set; } 
        public bool Retain { get; private set; } 
    }
}
