using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpEsp8266App.Services
{
    public class MqttService
    {
        private readonly IMqttClient _mqttClient;
        private MqttClientOptions? _mqttClientOptions;

        public MqttService()
        {
            var mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
            // Use builder classes where possible
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("broker.hivemq.com").Build();

            
        }

        public async Task<string> ConnectAsync(string brokerUrl, string clientId, CancellationToken cancellationToken = default)
        {
            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerUrl)
                .WithClientId(clientId)
                .WithCleanSession()
                .Build();

            try
            {
                await _mqttClient.ConnectAsync(_mqttClientOptions, cancellationToken);
                return "Connected to MQTT broker successfully.";
            }
            catch (Exception ex)
            {
                return $"Error connecting to MQTT broker: {ex.Message}";
            }
        }

        public async Task<string> PublishAsync(string topic, string payload, int qos = 0, CancellationToken cancellationToken = default)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                return "MQTT client is not connected.";
            }

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(false)
                .Build();

            try
            {
                await _mqttClient.PublishAsync(applicationMessage, cancellationToken);
                return $"Message published to topic '{topic}': {payload}";
            }
            catch (Exception ex)
            {
                return $"Error publishing message: {ex.Message}";
            }
        }

        public async Task<string> DisconnectAsync()
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                return "MQTT client is already disconnected.";
            }

            try
            {
                await _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.ImplementationSpecificError);
                return "Disconnected from MQTT broker cleanly.";
            }
            catch (Exception ex)
            {
                return $"Error during disconnect: {ex.Message}";
            }
        }
    }
}
