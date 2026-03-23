using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class AgregarEventoPage : ContentPage
{
    AuditoriaService service = new AuditoriaService();

    public AgregarEventoPage()
    {
        InitializeComponent();
    }

    private async void GuardarEvento(object sender, EventArgs e)
    {
        var evento = new EventoAuditoria
        {
            Fecha = FechaInicio.Date,
            Tipo = TipoPicker.SelectedItem?.ToString(),
            Titulo = TipoPicker.SelectedItem?.ToString(),
            Descripcion = ComentariosEntry.Text
        };

        await service.CrearEventoAsync(evento);

        await DisplayAlert("OK", "Guardado", "OK");

        await Navigation.PopAsync();
    }
}