using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class AgregarNotaPage : ContentPage
{
    public AgregarNotaPage()
    {
        InitializeComponent();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TituloEntry.Text) ||
            string.IsNullOrWhiteSpace(DescripcionEditor.Text))
        {
            await DisplayAlert("Error", "Completa todos los campos", "OK");
            return;
        }

        var nuevaNota = new Nota
        {
            Titulo = TituloEntry.Text,
            Descripcion = DescripcionEditor.Text
        };

        // Guardar en memoria
        NotasService.Notas.Add(nuevaNota);

        await Navigation.PopAsync();
    }
}