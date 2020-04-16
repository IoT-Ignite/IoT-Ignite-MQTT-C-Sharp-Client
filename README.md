# IoT-Ignite MQTT Client Example for C#

IoT-Ignite MQTT Client can be used for connecting gateways to IoT-Ignite cloud with MQTT Protocol. 

This project includes 3 main components:

- Typed classes to match the JSON messages used by IoT-Ignite MQTT Client 
- Basic Example API to send data and read an On/Off button from an IoT-Ignite Dashboard
- Example code how to use the API

Dependencies on Libraries:
- Microsoft.NETCore 2.2.0
- Newtonsoft.Json 12.0.3
- MQTTnet 3.0.8
- System.Configuration.ConfigurationManager 4.7.0

MQTT credentials must be changed in the "app.config":

```
    <add key="Username" value="MyUsername" />
    <add key="Password" value="MyPassword" />
    <add key="ClientId" value="MyClientId" />
```

These credentials can be generated from [Devzone Console](https://devzone.iot-ignite.com/dpanel). 

For more information about MQTT Client please visit [Devzone documentation](https://devzone.iot-ignite.com/knowledge-base/using-iot-ignite-mqtt-client/)