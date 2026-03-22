using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class SolicitarFixturePage : ContentPage
{
    string estadoFixture = "";
    bool fotoTomada = false;

    FixtureItem fixtureActual;

    public SolicitarFixturePage(FixtureItem fixture)
    {
        InitializeComponent();

        fixtureActual = fixture;

        CargarFixtureDesdeCatalogo();
    }

    void CargarFixtureDesdeCatalogo()
    {
        if (fixtureActual == null)
            return;

        FixtureCard.IsVisible = true;

        SerialLabel.Text = fixtureActual.Serial;
        ProcesoLabel.Text = fixtureActual.Proceso;

        estadoFixture = fixtureActual.Estado;

        EstadoLabel.Text = estadoFixture;

        if (estadoFixture == "Disponible")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#2E7D32");
            ConfirmarButton.IsEnabled = true;
        }
        else if (estadoFixture == "En uso")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#F9A825");
            ConfirmarButton.IsEnabled = false;

            DisplayAlert(
                "Fixture no disponible",
                "Esta fixture se encuentra actualmente en uso.",
                "OK");
        }
        else if (estadoFixture == "Dañada")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#C62828");
            ConfirmarButton.IsEnabled = false;

            DisplayAlert(
                "Fixture no disponible",
                "La fixture está dañada.",
                "OK");
        }
    }

    async void EscanearCodigo(object sender, EventArgs e)
    {
#if WINDOWS
        await DisplayAlert(
            "Escáner",
            "El escaneo solo está disponible en Android.",
            "OK");
        return;
#endif

        // Simulación de escaneo
        string codigo = "FX-1001";

        CargarFixture(codigo);
    }

    void CargarFixture(string codigo)
    {
        FixtureCard.IsVisible = true;

        SerialLabel.Text = codigo;
        ProcesoLabel.Text = "Sellador";

        estadoFixture = "Disponible";

        EstadoLabel.Text = estadoFixture;

        if (estadoFixture == "Disponible")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#2E7D32");
            ConfirmarButton.IsEnabled = true;
        }
        else if (estadoFixture == "En uso")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#F9A825");
            ConfirmarButton.IsEnabled = false;

            DisplayAlert(
                "Fixture no disponible",
                "Esta fixture se encuentra actualmente en uso.",
                "OK");
        }
        else if (estadoFixture == "Dañada")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#C62828");
            ConfirmarButton.IsEnabled = false;

            DisplayAlert(
                "Fixture no disponible",
                "La fixture está dañada.",
                "OK");
        }
    }

    async void TomarFoto(object sender, EventArgs e)
    {
#if WINDOWS
        await DisplayAlert(
            "Cámara",
            "La cámara solo está disponible en Android.",
            "OK");
        return;
#endif

        try
        {
            var foto = await MediaPicker.CapturePhotoAsync();

            if (foto != null)
            {
                fotoTomada = true;

                await DisplayAlert(
                    "Foto capturada",
                    "La fotografía fue tomada correctamente.",
                    "OK");
            }
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "No fue posible acceder a la cámara.",
                "OK");
        }
    }

    async void ConfirmarSolicitud(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(UsuarioEntry.Text))
        {
            await DisplayAlert(
                "Error",
                "Ingrese el nombre del usuario.",
                "OK");
            return;
        }

        if (!fotoTomada)
        {
            await DisplayAlert(
                "Error",
                "Debe tomar una fotografía de evidencia.",
                "OK");
            return;
        }

        await DisplayAlert(
            "Solicitud registrada",
            "La fixture fue solicitada correctamente.",
            "OK");

        await Navigation.PopAsync();
    }
}