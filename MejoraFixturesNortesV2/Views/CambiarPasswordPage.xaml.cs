namespace MejoraFixturesNortesV2.Views;

public partial class CambiarPasswordPage : ContentPage
{
    bool nuevaVisible = false;
    bool confirmarVisible = false;

    public CambiarPasswordPage()
    {
        InitializeComponent();
    }

    private void OnToggleNuevaPassword(object sender, EventArgs e)
    {
        nuevaVisible = !nuevaVisible;
        NuevaPasswordEntry.IsPassword = !nuevaVisible;
    }

    private void OnToggleConfirmarPassword(object sender, EventArgs e)
    {
        confirmarVisible = !confirmarVisible;
        ConfirmarPasswordEntry.IsPassword = !confirmarVisible;
    }

    private async void OnActualizarPassword(object sender, EventArgs e)
    {
        await DisplayAlert("Sistema", "Función en desarrollo", "OK");
    }

    private async void OnVolverLogin(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}