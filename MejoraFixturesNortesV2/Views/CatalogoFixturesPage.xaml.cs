using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;
using System.Collections.ObjectModel;

namespace MejoraFixturesNortesV2.Views;

public partial class CatalogoFixturesPage : ContentPage
{
    readonly FixtureDatabase _db;
    public ObservableCollection<FixtureItem> Fixtures { get; } = new();
    bool esAdmin;
    public bool EsAdmin => esAdmin;

    // Usuario normal
    public CatalogoFixturesPage()
    {
        InitializeComponent();
        esAdmin = false;
        _db = Handler?.MauiContext?.Services.GetService<FixtureDatabase>()
              ?? IPlatformApplication.Current.Services.GetService<FixtureDatabase>();
        Inicializar();
    }

    // Admin
    public CatalogoFixturesPage(bool esAdmin)
    {
        InitializeComponent();
        this.esAdmin = esAdmin;
        _db = IPlatformApplication.Current.Services.GetService<FixtureDatabase>();
        Inicializar();
    }

    void Inicializar()
    {
        BindingContext = this;
        FixturesCollection.ItemsSource = Fixtures;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarFixturesAsync();
    }

    async Task CargarFixturesAsync()
    {
        var lista = await _db.ObtenerTodosAsync();
        Fixtures.Clear();
        foreach (var f in lista)
            Fixtures.Add(f);
    }

    // Buscar
    private void OnBuscarTextChanged(object sender, TextChangedEventArgs e)
    {
        string texto = e.NewTextValue?.ToLower() ?? "";
        FixturesCollection.ItemsSource = Fixtures
            .Where(f =>
                f.Serial.ToLower().Contains(texto) ||
                f.Proceso.ToLower().Contains(texto))
            .ToList();
    }

    // Solicitar (usuario normal)
    private async void OnSolicitarClicked(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;
        if (fixture == null) return;

        if (fixture.Estado == "Disponible")
        {
            await Navigation.PushAsync(new SolicitarFixturePage(fixture));
            return;
        }

        await DisplayAlert(
            "Fixture no disponible",
            "Esta fixture no se encuentra disponible en este momento.\n\n" +
            "Si requiere utilizar la fixture para una pieza ACT, favor de comunicarse con el líder del área.",
            "Entendido");
    }

    // Cambiar estado (Admin)
    private async void OnCambiarEstadoClicked(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;
        if (fixture == null) return;

        string nuevoEstado = await DisplayActionSheet(
            "Cambiar estado", "Cancelar", null,
            "Disponible", "En uso", "Dañada", "Baja");

        if (nuevoEstado == "Cancelar" || string.IsNullOrEmpty(nuevoEstado)) return;

        fixture.Estado = nuevoEstado;
        await _db.GuardarAsync(fixture);

        FixturesCollection.ItemsSource = null;
        FixturesCollection.ItemsSource = Fixtures;
    }

    // Eliminar (Admin)
    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;
        if (fixture == null) return;

        bool confirmar = await DisplayAlert(
            "Eliminar", $"¿Eliminar {fixture.Serial}?", "Sí", "No");

        if (confirmar)
        {
            await _db.EliminarAsync(fixture);
            Fixtures.Remove(fixture);
        }
    }

    // Cerrar sesión
    private void CerrarSesion(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}