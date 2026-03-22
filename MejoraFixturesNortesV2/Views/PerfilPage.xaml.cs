using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class PerfilPage : ContentPage
{

    public PerfilPage()
    {
        InitializeComponent();

        // 🔧 DATOS DE PRUEBA (puedes quitar después)
        FixturesCollection.ItemsSource = new List<FixtureItem>
        {
            new FixtureItem
            {
                Serial = "FX-1002",
                Proceso = "Nutplates",
                Imagen = "fx1002.png"
            }
        };

        CargarResumen();
    }

    // 🔵 RESUMEN
    void CargarResumen()
    {
        try
        {
            var lista = FixturesCollection.ItemsSource as IEnumerable<FixtureItem>;

            int activos = lista != null ? lista.Count() : 0;
            int pendientes = 0;

            LblActivos.Text = $"🔧 {activos} fixtures activos";
            LblPendientes.Text = $"⏳ {pendientes} pendientes";
            LblUltimo.Text = $"🕒 Último movimiento: hoy";
        }
        catch
        {
            LblActivos.Text = "🔧 0 fixtures activos";
            LblPendientes.Text = "⏳ 0 pendientes";
            LblUltimo.Text = "🕒 Sin información";
        }
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
        bool aceptar = await DisplayAlert(
            "Fixture dañada",
            "Favor de dirigirse con el coordinador.\n\nSi la fixture está dañada se levantará un reporte de incidencia.",
            "Acepto responsabilidad",
            "Cancelar");

        if (aceptar)
        {
            await Navigation.PushAsync(new DanosPage());
        }
    }

    // ❌ PERDIDA
    async void MarcarPerdida(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Confirmar pérdida",
            "¿Seguro que esta fixture se ha perdido?",
            "Sí",
            "No");

        if (confirmar)
        {
            await DisplayAlert(
                "Fixture perdida",
                "La fixture ha sido marcada como perdida.",
                "OK");
        }
    }

    // 🔒 CERRAR SESION
    void CerrarSesion(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}