using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpEsp8266App;

public partial class MainPage : ContentPage
{
    private UdpClient? _udpClient;
    private bool _isListening;
    private readonly Queue<string> _messageQueue = new();

    public MainPage()
    {
        InitializeComponent();
        RemoteIpEntry.Text = Preferences.Get("LastIp", string.Empty);
        RemotePortEntry.Text = Preferences.Get("LastRemotePort", "5000");
        LocalPortEntry.Text = Preferences.Get("LastLocalPort", "5000");
    }

    private void SaveLastEntries()
    {
        Preferences.Set("LastIp", RemoteIpEntry.Text);
        Preferences.Set("LastLocalPort", LocalPortEntry.Text);
        Preferences.Set("LastRemotePort", RemotePortEntry.Text);
    }
    private async void OnStartConnectionClicked(object sender, EventArgs e)
    {
        if (_udpClient != null)
        {
            await DisplayAlert("Error", "Connection already started.", "OK");
            return;
        }

        try
        {
            SaveLastEntries();
            int localPort = int.Parse(LocalPortEntry.Text);
            int remotePort = int.Parse(RemotePortEntry.Text);
            string remoteIp = RemoteIpEntry.Text;

            _udpClient = new UdpClient(localPort);
            _udpClient.Connect(remoteIp, remotePort);

            _isListening = true;
            StartListening();

            await DisplayAlert("Success", "UDP connection started.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void StartListening()
    {
        if (_udpClient == null) return;
        await Task.Run(async () =>
        {
            try
            {
                while (_isListening)
                {
                    var result = await _udpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);

                    // Add timestamp and trim CR LF for display
                    string timestamp = DateTime.Now.ToString("HH:mm:ss");
                    string formattedMessage = $"{timestamp} - {message.Trim('\r', '\n')}";

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateTerminal(formattedMessage);
                    });
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateTerminal($"Error: {ex.Message}");
                });
            }
        });
    }

    private void UpdateTerminal(string newMessage)
    {
        _messageQueue.Enqueue(newMessage);
        if (_messageQueue.Count > 6)
        {
            _messageQueue.Dequeue();
        }

        var messages = _messageQueue.ToArray();
        Line1.Text = messages.Length > 0 ? messages[0] : string.Empty;
        Line2.Text = messages.Length > 1 ? messages[1] : string.Empty;
        Line3.Text = messages.Length > 2 ? messages[2] : string.Empty;
        Line4.Text = messages.Length > 3 ? messages[3] : string.Empty;
        Line5.Text = messages.Length > 4 ? messages[4] : string.Empty;
        Line6.Text = messages.Length > 5 ? messages[5] : string.Empty;
    }

    private void OnStopConnectionClicked(object sender, EventArgs e)
    {
        _isListening = false;
        _udpClient?.Dispose();
        _udpClient = null;

        UpdateTerminal("[Info] Connection stopped.");
    }

    private void OnClearScreenClicked(object sender, EventArgs e)
    {
        _messageQueue.Clear();
        UpdateTerminal(string.Empty);
    }

    private async void OnSendMessageClicked(object sender, EventArgs e)
    {
        if (_udpClient == null)
        {
            await DisplayAlert("Error", "Connection not started yet.", "OK");
            return;
        }

        try
        {
            string message = MessageEntry.Text + "\r\n"; // Add CR LF
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(data, data.Length);

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string formattedMessage = $"[Sent] {timestamp} - {MessageEntry.Text}";
            UpdateTerminal(formattedMessage);
        }
        catch (Exception ex)
        {
            UpdateTerminal($"Error sending message: {ex.Message}");
        }
    }
}
