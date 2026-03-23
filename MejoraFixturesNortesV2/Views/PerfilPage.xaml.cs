using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class PerfilPage : ContentPage
{
    readonly FixtureDatabase _db = null!;

    public PerfilPage()
    {
        InitializeComponent();
        _db = IPlatformApplication.Current!.Services.GetService<FixtureDatabase>()!;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var usuario = SesionService.UsuarioActual;
        if (usuario != null)
        {
            NombreLabel.Text = usuario.Nombre;
            EmpleadoLabel.Text = usuario.NumeroEmpleado;
        }

        await CargarFixturesAsync();
    }

    async Task CargarFixturesAsync()
    {
        var usuario = SesionService.UsuarioActual;
        if (usuario == null) return;

        List<FixtureItem> lista;

        if (usuario.EsSupervisor)
            lista = await _db.ObtenerTodasFixturesEnUsoAsync();
        else
            lista = await _db.ObtenerFixturesEnUsoDeUsuarioAsync(usuario.Nombre);

        FixturesCollection.ItemsSource = lista;
        CargarResumen(lista);
    }

    void CargarResumen(List<FixtureItem> lista)
    {
        int activos = lista?.Count ?? 0;
        LblActivos.Text = $"🔧 {activos} fixtures activos";
        LblPendientes.Text = $"⏳ 0 pendientes";
        LblUltimo.Text = $"🕒 Último movimiento: hoy";
    }

    // 🔧 DEVOLVER
    async void DevolverFixture(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;

        if (fixture == null)
            return;

        await Navigation.PushAsync(new DevolverFixturePage(fixture));
    }

    // ⚠️ DAÑADA
    async void MarcarDanada(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;
        if (fixture == null) return;

        bool aceptar = await DisplayAlert(
            "Fixture dañada",
            "Favor de dirigirse con el coordinador.\n\nSi la fixture está dañada se levantará un reporte de incidencia.",
            "Acepto responsabilidad",
            "Cancelar");

        if (aceptar)
            await Navigation.PushAsync(new DanosPage(fixture.Serial));
    }

    // ❌ PERDIDA
    async void MarcarPerdida(object sender, EventArgs e)
    {
        var boton = sender as Button;
        var fixture = boton?.BindingContext as FixtureItem;
        if (fixture == null) return;

        bool confirmar = await DisplayAlert(
            "Confirmar pérdida",
            $"¿Seguro que la fixture {fixture.Serial} se ha perdido?",
            "Sí",
            "No");

        if (!confirmar) return;

        // Registrar movimiento
        await _db.RegistrarMovimientoAsync(new MovimientoFixture
        {
            FixtureSerial = fixture.Serial,
            Tipo = "Pérdida",
            Usuario = SesionService.UsuarioActual?.Nombre ?? "Desconocido",
            Observaciones = "Reportada como perdida desde perfil",
            Fecha = DateTime.Now
        });

        // Actualizar estado en BD
        fixture.Estado = "Baja";
        fixture.Responsable = "Sin asignar";
        await _db.GuardarAsync(fixture);

        await DisplayAlert(
            "Fixture perdida",
            $"La fixture {fixture.Serial} ha sido marcada como baja.",
            "OK");

        await CargarFixturesAsync();
    }

    // 🔒 CERRAR SESION
    void CerrarSesion(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}