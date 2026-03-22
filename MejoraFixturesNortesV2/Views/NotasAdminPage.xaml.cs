using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class NotasAdminPage : ContentPage
{
    public NotasAdminPage()
    {
        InitializeComponent();

        NotasList.ItemsSource = NotasService.Notas;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Refrescar lista
        NotasList.ItemsSource = null;
        NotasList.ItemsSource = NotasService.Notas;
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgregarNotaPage());
    }

    private void OnEliminarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var nota = button?.CommandParameter as Nota;

        if (nota != null)
        {
            NotasService.Notas.Remove(nota);
        }
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}