using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class DevolverFixturePage : ContentPage
{
    FixtureItem fixtureActual;

    bool fotoTomada = false;

    // Constructor cuando se abre desde el menú principal
    public DevolverFixturePage()
    {
        InitializeComponent();

        CargarFixturesEnUso();
    }

    // Constructor cuando se abre desde otra página con una fixture específica
    public DevolverFixturePage(FixtureItem fixture)
    {
        InitializeComponent();

        fixtureActual = fixture;

        CargarFixture();
    }

    void CargarFixturesEnUso()
    {
        // Simulación temporal hasta conectar con API o BD

        fixtureActual = new FixtureItem
        {
            Serial = "FX-1002",
            Proceso = "Nutplates",
            Responsable = "Juan Pérez",
            Estado = "En uso"
        };

        CargarFixture();
    }

    void CargarFixture()
    {
        if (fixtureActual == null)
            return;

        SerialLabel.Text = fixtureActual.Serial;

        ProcesoLabel.Text = fixtureActual.Proceso;

        ResponsableLabel.Text = $"Responsable: {fixtureActual.Responsable}";

        EstadoLabel.Text = fixtureActual.Estado;

        if (fixtureActual.Estado == "En uso")
            EstadoLabel.BackgroundColor = Color.FromArgb("#F9A825");
    }

    async void TomarFoto(object sender, EventArgs e)
    {

#if WINDOWS
        await DisplayAlert(
            "Cámara",
            "La cámara solo funciona en dispositivos móviles.",
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
                    "Fotografía capturada",
                    "La evidencia fue tomada correctamente.",
                    "OK");
            }
        }
        catch
        {
            await DisplayAlert(
                "Error",
                "No fue posible abrir la cámara.",
                "OK");
        }
    }

    async void ConfirmarDevolucion(object sender, EventArgs e)
    {
        if (!fotoTomada)
        {
            await DisplayAlert(
                "Fotografía requerida",
                "Debe tomar una fotografía de la fixture antes de devolverla.",
                "OK");

            return;
        }

        string observaciones = ObservacionesEntry.Text;

        // Aquí irá la llamada a BD o API
        // Ejemplo futuro:
        // await fixtureService.DevolverFixture(fixtureActual.Serial, observaciones);

        await DisplayAlert(
            "Devolución registrada",
            "La fixture fue devuelta correctamente.",
            "OK");

        await Navigation.PopAsync();
    }
}