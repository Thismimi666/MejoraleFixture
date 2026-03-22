using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Services;
using MejoraFixturesNortesV2.Views;

namespace MejoraFixturesNortesV2.Views;

public partial class LoginPage : ContentPage
{
    bool passwordVisible = false;
    readonly FixtureDatabase _db;

    public LoginPage()
    {
        InitializeComponent();
        _db = IPlatformApplication.Current.Services.GetService<FixtureDatabase>();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string empleado = UsuarioEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(empleado) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Ingrese su número de empleado y contraseña", "OK");
            return;
        }

        var usuario = await _db.AutenticarAsync(empleado, password);
        if (usuario == null)
        {
            await DisplayAlert("Acceso denegado", "Número de empleado o contraseña incorrectos.", "OK");
            return;
        }

        SesionService.UsuarioActual = usuario;

        if (usuario.EsAdmin)
            await Navigation.PushAsync(new AdminPage());
        else
            await Navigation.PushAsync(new MenuPrincipalPage());
    }

    private void OnTogglePassword(object sender, EventArgs e)
    {
        passwordVisible = !passwordVisible;
        PasswordEntry.IsPassword = !passwordVisible;
    }

    private async void OnCambiarPassword(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CambiarPasswordPage());
    }

    private async void AbrirGraficos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GraficosAuditoriaPage());
    }
}