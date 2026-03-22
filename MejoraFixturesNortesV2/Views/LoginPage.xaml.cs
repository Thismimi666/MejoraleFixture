using MejoraFixturesNortesV2.Views;

namespace MejoraFixturesNortesV2.Views;

public partial class LoginPage : ContentPage
{
    bool passwordVisible = false;

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string empleado = UsuarioEntry.Text?.Trim();

        // Validación básica
        if (string.IsNullOrEmpty(empleado))
        {
            await DisplayAlert("Error", "Ingrese su número de empleado", "OK");
            return;
        }

        // 🔐 LOGIN ADMIN (temporal)
        if (empleado.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new AdminPage());
            return;
        }

        // 👤 LOGIN NORMAL
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