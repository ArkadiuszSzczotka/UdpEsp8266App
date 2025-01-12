using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpEsp8266App;

public partial class TcpPage : ContentPage
{
    private TcpListener? _tcpListener;
    private TcpClient? _connectedClient;
    private NetworkStream? _networkStream;
    private CancellationTokenSource? _cts;
    private readonly Queue<string> _messageQueue = new();
    public NetworkStream? NetworkStream => _networkStream;
    public TcpPage()
    {
        InitializeComponent();
        ClientIp.Text = Preferences.Get("LastClientIp", string.Empty);
        ServerPortEntry.Text = Preferences.Get("LastServerPort", "8000");
    }

    private void SaveLastEntries()
    {
        Preferences.Set("LastClientIp", ClientIp.Text);
        Preferences.Set("LastServerPort", ServerPortEntry.Text);
    }

    private async void OnStartServerClicked(object sender, EventArgs e)
    {
        if (_tcpListener != null)
        {
            await DisplayAlert("Error", "Server is already running.", "OK");
            return;
        }

        try
        {
            SaveLastEntries();

            int port = int.Parse(ServerPortEntry.Text);

            // Start the server on all available IPs (IPAddress.Any)
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _cts = new CancellationTokenSource();

            UpdateTerminal($"[Info] Server started on port {port}");

            AcceptClientsAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void AcceptClientsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await _tcpListener!.AcceptTcpClientAsync(token);

                string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
                if (clientIp == ClientIp.Text)
                {
                    // Accept connection
                    _connectedClient = client;
                    _networkStream = _connectedClient.GetStream();

                    UpdateTerminal($"[Info] Client connected: {clientIp}");

                    ReceiveMessagesAsync(_networkStream, token);
                }
                else
                {
                    UpdateTerminal($"[Info] Connection rejected from: {clientIp}");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                UpdateTerminal($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async void ReceiveMessagesAsync(NetworkStream stream, CancellationToken token)
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (!token.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string timestamp = DateTime.Now.ToString("HH:mm:ss");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateTerminal($"[Client] {timestamp} - {message}");
                    });
                }
            }
        }
        catch (Exception ex)
        {
            UpdateTerminal($"Error receiving message: {ex.Message}");
        }
    }

    private async void OnSendMessageClicked(object sender, EventArgs e)
    {
        if (_networkStream == null || !_networkStream.CanWrite)
        {
            await DisplayAlert("Error", "No client connected or stream unavailable.", "OK");
            return;
        }

        try
        {
            string message = ServerMessageEntry.Text + "\r\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _networkStream.WriteAsync(data, 0, data.Length);

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            UpdateTerminal($"[Sent] {timestamp} - {ServerMessageEntry.Text}");
        }
        catch (Exception ex)
        {
            UpdateTerminal($"Error sending message: {ex.Message}");
        }
    }

    private void OnStopServerClicked(object sender, EventArgs e)
    {
        try
        {
            _cts?.Cancel();
            _networkStream?.Close();
            _connectedClient?.Close();
            _tcpListener?.Stop();

            _tcpListener = null;
            _connectedClient = null;
            _networkStream = null;

            UpdateTerminal("[Info] Server stopped.");
        }
        catch (Exception ex)
        {
            UpdateTerminal($"Error stopping server: {ex.Message}");
        }
    }

    private void OnClearScreenClicked(object sender, EventArgs e)
    {
        _messageQueue.Clear();
        UpdateTerminal(string.Empty);
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


}
