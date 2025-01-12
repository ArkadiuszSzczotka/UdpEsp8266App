using UdpEsp8266App.Models;
namespace UdpEsp8266App;

public partial class SwitchControlPage : ContentPage
{
    private readonly SwitchControlViewModel _viewModel;

    public SwitchControlPage(TcpPage tcpPage)
    {
        InitializeComponent();
        _viewModel = new SwitchControlViewModel(tcpPage);
        BindingContext = _viewModel;
    }

    private async void OnConnectToMqttClicked(object sender, EventArgs e)
    {
        string result = await _viewModel.ConnectToMqttBrokerAsync();
        await DisplayAlert("MQTT Connection", result, "OK");
    }
    private async void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch switchControl && switchControl.BindingContext is SwitchModel switchModel)
        {
            string result = await _viewModel.SendStateAsync(switchModel);
            await DisplayAlert("Result", result, "OK");
        }
    }

}
