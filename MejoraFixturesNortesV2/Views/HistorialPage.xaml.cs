using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class HistorialPage : ContentPage
{
    readonly FixtureDatabase _db = null!;

    public HistorialPage()
    {
        InitializeComponent();
        _db = IPlatformApplication.Current!.Services.GetService<FixtureDatabase>()!;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarHistorialAsync();
    }

    async Task CargarHistorialAsync()
    {
        var movimientos = await _db.ObtenerTodosMovimientosAsync();
        MovimientosCollection.ItemsSource = movimientos
            .OrderByDescending(m => m.Fecha)
            .ToList();
    }

    async void VolverButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void VerFoto(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var movimiento = boton?.BindingContext as MovimientoFixture;

        if (movimiento == null || string.IsNullOrEmpty(movimiento.FotoRuta))
        {
            await DisplayAlert("Sin foto", "No hay fotografía registrada para este movimiento.", "OK");
            return;
        }

        if (!File.Exists(movimiento.FotoRuta))
        {
            await DisplayAlert("Sin foto", "El archivo de fotografía no se encontró en el dispositivo.", "OK");
            return;
        }

        FotoPopupImage.Source = ImageSource.FromFile(movimiento.FotoRuta);
        FotoOverlay.IsVisible = true;
    }

    void CerrarFotoOverlay(object sender, TappedEventArgs e)
    {
        FotoOverlay.IsVisible = false;
        FotoPopupImage.Source = null;
    }
}
