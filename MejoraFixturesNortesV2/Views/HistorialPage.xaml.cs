using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class HistorialPage : ContentPage
{
    readonly FixtureDatabase _db = null!;
    List<MovimientoFixture> _todos = new();
    string? _filtroTipo = null; // null=Todos | "Daños" | "Bajas" | "Daños+Bajas"

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

    void Chip_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn == ChipTodos) _filtroTipo = null;
        else if (btn == ChipDanos) _filtroTipo = "Daños";
        else if (btn == ChipBajas) _filtroTipo = "Bajas";
        else _filtroTipo = "Daños+Bajas";

        ActualizarChips();
        AplicarFiltro(BuscadorEntry.Text);
    }

    void ActualizarChips()
    {
        var inactivo = Color.FromArgb("#E0E0E0");
        var textoInact = Color.FromArgb("#444444");

        ChipTodos.BackgroundColor = inactivo; ChipTodos.TextColor = textoInact;
        ChipDanos.BackgroundColor = inactivo; ChipDanos.TextColor = textoInact;
        ChipBajas.BackgroundColor = inactivo; ChipBajas.TextColor = textoInact;
        ChipAmbos.BackgroundColor = inactivo; ChipAmbos.TextColor = textoInact;

        switch (_filtroTipo)
        {
            case null:
                ChipTodos.BackgroundColor = Color.FromArgb("#0D3B66");
                ChipTodos.TextColor = Colors.White;
                break;
            case "Daños":
                ChipDanos.BackgroundColor = Color.FromArgb("#C62828");
                ChipDanos.TextColor = Colors.White;
                break;
            case "Bajas":
                ChipBajas.BackgroundColor = Color.FromArgb("#4A148C");
                ChipBajas.TextColor = Colors.White;
                break;
            default:
                ChipAmbos.BackgroundColor = Color.FromArgb("#0D3B66");
                ChipAmbos.TextColor = Colors.White;
                break;
        }
    }

    async void AbrirGrafica(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GraficaDanosPage());
    }

    void AplicarFiltro(string? texto)
    {
        IEnumerable<MovimientoFixture> resultado = _todos;

        // Filtro por tipo
        resultado = _filtroTipo switch
        {
            "Daños" => resultado.Where(m => m.Tipo == "Daño"),
            "Bajas" => resultado.Where(m => m.Tipo == "Pérdida"),
            "Daños+Bajas" => resultado.Where(m => m.Tipo == "Daño" || m.Tipo == "Pérdida"),
            _ => resultado
        };

        // Filtro por texto
        if (!string.IsNullOrWhiteSpace(texto))
        {
            var q = texto.Trim().ToLowerInvariant();
            resultado = resultado.Where(m =>
                (m.FixtureSerial?.ToLowerInvariant().Contains(q) ?? false) ||
                (m.Usuario?.ToLowerInvariant().Contains(q) ?? false) ||
                (m.Tipo?.ToLowerInvariant().Contains(q) ?? false) ||
                (m.Observaciones?.ToLowerInvariant().Contains(q) ?? false)
            );
        }

        var lista = resultado.ToList();
        MovimientosCollection.ItemsSource = null;
        MovimientosCollection.ItemsSource = lista;
        ContadorLabel.Text = lista.Count == _todos.Count
            ? $"{_todos.Count}"
            : $"{lista.Count}/{_todos.Count}";
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
