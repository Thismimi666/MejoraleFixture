using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class HistorialPage : ContentPage
{
    readonly FixtureDatabase _db = null!;
    List<MovimientoFixture> _todos = new();

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
        _todos = (await _db.ObtenerTodosMovimientosAsync())
            .OrderByDescending(m => m.Fecha)
            .ToList();

        AplicarFiltro(BuscadorEntry.Text);
    }

    void Buscar_TextChanged(object sender, TextChangedEventArgs e)
    {
        AplicarFiltro(e.NewTextValue);
    }

    void AplicarFiltro(string? texto)
    {
        List<MovimientoFixture> resultado;

        if (string.IsNullOrWhiteSpace(texto))
        {
            resultado = _todos;
        }
        else
        {
            var q = texto.Trim().ToLowerInvariant();
            resultado = _todos.Where(m =>
                (m.FixtureSerial?.ToLowerInvariant().Contains(q) ?? false) ||
                (m.Usuario?.ToLowerInvariant().Contains(q) ?? false) ||
                (m.Tipo?.ToLowerInvariant().Contains(q) ?? false) ||
                (m.Observaciones?.ToLowerInvariant().Contains(q) ?? false)
            ).ToList();
        }

        MovimientosCollection.ItemsSource = null;
        MovimientosCollection.ItemsSource = resultado;
        ContadorLabel.Text = resultado.Count == _todos.Count
            ? $"{_todos.Count}"
            : $"{resultado.Count}/{_todos.Count}";
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
