using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;
using System.Collections.ObjectModel;

namespace MejoraFixturesNortesV2.Views;

public partial class CatalogoFixturesPage : ContentPage
{
    // 🔥 Datos
    public ObservableCollection<FixtureItem> Fixtures => DataService.Fixtures;

    // 🔥 Control de rol
    bool esAdmin;

    public bool EsAdmin => esAdmin;

    // 👤 Usuario normal
    public CatalogoFixturesPage()
    {
        InitializeComponent();

        esAdmin = false;

        Inicializar();
    }

    // 🛠 Admin
    public CatalogoFixturesPage(bool esAdmin)
    {
        InitializeComponent();

        this.esAdmin = esAdmin;

        Inicializar();
    }

    void Inicializar()
    {
        BindingContext = this;

        CargarFixtures();

        FixturesCollection.ItemsSource = Fixtures;
    }

    void CargarFixtures()
    {
        if (Fixtures.Count > 0)
            return;

        Fixtures.Add(new FixtureItem
        {
            Serial = "FX-1001",
            Proceso = "Sellador",
            Responsable = "Sin asignar",
            Estado = "Disponible",
            Imagen = "fixture1.png"
        });

        Fixtures.Add(new FixtureItem
        {
            Serial = "FX-1002",
            Proceso = "Nutplates",
            Responsable = "Juan Pérez",
            Estado = "En uso",
            Imagen = "fixture2.png"
        });

        Fixtures.Add(new FixtureItem
        {
            Serial = "FX-1003",
            Proceso = "Torque",
            Responsable = "Sin asignar",
            Estado = "Dañada",
            Imagen = "fixture3.png"
        });

        Fixtures.Add(new FixtureItem
        {
            Serial = "FX-1004",
            Proceso = "Tierras",
            Responsable = "Sin asignar",
            Estado = "Baja",
            Imagen = "fixture4.png"
        });
    }

    // 🔍 Buscar
    private void OnBuscarTextChanged(object sender, TextChangedEventArgs e)
    {
        string texto = e.NewTextValue?.ToLower() ?? "";

        FixturesCollection.ItemsSource = Fixtures
            .Where(f =>
                f.Serial.ToLower().Contains(texto) ||
                f.Proceso.ToLower().Contains(texto))
            .ToList();
    }

    // 📦 Solicitar (usuario normal)
    private async void OnSolicitarClicked(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;

        if (fixture == null)
            return;

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

    // 🛠 CAMBIAR ESTADO (ADMIN)
    private async void OnCambiarEstadoClicked(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;

        if (fixture == null)
            return;

        string nuevoEstado = await DisplayActionSheet(
            "Cambiar estado",
            "Cancelar",
            null,
            "Disponible",
            "En uso",
            "Dañada",
            "Baja");

        if (nuevoEstado == "Cancelar")
            return;

        fixture.Estado = nuevoEstado;

        // 🔥 refrescar UI
        FixturesCollection.ItemsSource = null;
        FixturesCollection.ItemsSource = Fixtures;
    }

    // 🗑 ELIMINAR (ADMIN)
    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;

        if (fixture == null)
            return;

        bool confirmar = await DisplayAlert(
            "Eliminar",
            $"¿Eliminar {fixture.Serial}?",
            "Sí",
            "No");

        if (confirmar)
        {
            Fixtures.Remove(fixture);
        }
    }

    // 🔐 Cerrar sesión
    private void CerrarSesion(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}