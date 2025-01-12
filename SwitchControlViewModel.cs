using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using MQTTnet;
using UdpEsp8266App.Models;

namespace UdpEsp8266App;

public partial class SwitchControlViewModel : ObservableObject
{
    private readonly TcpPage _tcpPage;
    public ObservableCollection<string> CommunicationTypes { get; } = new()
    {
        "TCP",
        "MQTT"
    };
    public ObservableCollection<int> QosOptions { get; } = new() { 0, 1, 2 };

    public ObservableCollection<SwitchModel> Switches { get; } = new();

    [ObservableProperty]
    private string selectedCommunicationType = "TCP"; // Default to TCP

    [ObservableProperty]
    private string mqttBrokerUrl = "test.mosquitto.org";

    [ObservableProperty]
    private string mqttTopic = "esp8266/switch";

    [ObservableProperty]
    private int mqttQos = 0;

    private IMqttClient? _mqttClient;

    public SwitchControlViewModel(TcpPage tcpPage)
    {
        _tcpPage = tcpPage;

        Switches.Add(new SwitchModel { Label = "Switch 1", State = false });
        Switches.Add(new SwitchModel { Label = "Switch 2", State = false });
        Switches.Add(new SwitchModel { Label = "Switch 3", State = false });
    }


    public async Task<string> ConnectToMqttBrokerAsync()
    {
        try
        {
            if (_mqttClient == null)
            {
                var factory = new MqttClientFactory();
                _mqttClient = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(MqttBrokerUrl)
                    .Build();

                await _mqttClient.ConnectAsync(options);
            }

            return "Connected to MQTT Broker successfully.";
        }
        catch (Exception ex)
        {
            return $"Error connecting to MQTT Broker: {ex.Message}";
        }
    }

    public async Task<string> SendStateAsync(SwitchModel switchModel)
    {
        string message = $"{switchModel.Label}: {(switchModel.State ? "ON" : "OFF")}";

        if (SelectedCommunicationType == "TCP")
        {
            return await SendViaTcpAsync(message);
        }
        else if (SelectedCommunicationType == "MQTT")
        {
            return await SendViaMqttAsync(message);
        }

        return "Unknown communication type.";
    }


    private async Task<string> SendViaTcpAsync(string message)
    {
        if (_tcpPage.NetworkStream == null)
        {
            return "TCP connection not established.";
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\r\n");
            await _tcpPage.NetworkStream.WriteAsync(data, 0, data.Length);
            return $"Sent TCP: {message}";
        }
        catch (Exception ex)
        {
            return $"[TCP] Error: {ex.Message}";
        }
    }
    private async Task<string> SendViaMqttAsync(string message)
    {
        try
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                var connectionResult = await ConnectToMqttBrokerAsync();
                if (!connectionResult.StartsWith("Connected"))
                {
                    return connectionResult;
                }
            }

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopic)
                .WithPayload(message)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)MqttQos)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage);
            return $"Message sent via MQTT: {message}";
        }
        catch (Exception ex)
        {
            return $"[MQTT] Error: {ex.Message}";
        }
    }
}

