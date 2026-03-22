using System;
using Microsoft.Maui.Controls;

namespace MejoraFixturesNortesV2.Views;

public partial class ModuloAdminPage : ContentPage
{
    public ModuloAdminPage()
    {
        InitializeComponent();
    }

    // 🔹 Agregar Fixture
    private async void AbrirAgregarFixture(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgregarFixturePage());
    }

    // 🔹 Agregar Evento al Calendario
    private async void AbrirAgregarEvento(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new AgregarEventoPage());
    }

    // 🔹 Agregar Alerta de Calidad
    private async void AbrirAgregarAlerta(object sender, EventArgs e)
    {
    {
        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("Alertadecalidad.pptx");

            var filePath = Path.Combine(FileSystem.CacheDirectory, "Alertadecalidad.pptx");

            using (var fileStream = File.Create(filePath))
            {
                await stream.CopyToAsync(fileStream);
            }

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
    }

    // 🔹 Gestionar Fixture (Editar / Eliminar)
    private async void AbrirGestionFixture(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatalogoFixturesPage(true));
    }

    // 🔹 Cerrar Sesión
    private async void CerrarSesion(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cerrar sesión", "¿Deseas salir?", "Sí", "No");

        if (confirm)
        {
            // Limpia navegación y regresa al login
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}