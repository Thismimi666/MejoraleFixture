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
        else if (estadoFixture == "Da�ada")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#C62828");
            ConfirmarButton.IsEnabled = false;

            DisplayAlert(
                "Fixture no disponible",
                "La fixture est� da�ada.",
                "OK");
        }
    }

    async void VolverButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void EscanearCodigo(object sender, EventArgs e)
    {
#if WINDOWS
        await DisplayAlert(
            "Esc�ner",
            "El escaneo solo est� disponible en Android.",
            "OK");
        return;
#endif

        // Simulaci�n de escaneo
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
        else if (estadoFixture == "Da�ada")
        {
            EstadoLabel.BackgroundColor = Color.FromArgb("#C62828");
            ConfirmarButton.IsEnabled = false;

            DisplayAlert(
                "Fixture no disponible",
                "La fixture est� da�ada.",
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

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert(
                "Permiso requerido",
                "Se necesita permiso de cámara para tomar la fotografía.",
                "OK");
            return;
        }

        try
        {
            var foto = await MediaPicker.CapturePhotoAsync();

            if (foto != null)
            {
                fotoTomada = true;

                await DisplayAlert(
                    "Foto capturada",
                    "La fotograf�a fue tomada correctamente.",
                    "OK");
            }
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "No fue posible acceder a la c�mara.",
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
                "Debe tomar una fotograf�a de evidencia.",
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