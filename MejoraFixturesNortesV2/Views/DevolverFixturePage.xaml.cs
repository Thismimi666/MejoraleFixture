using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Views;

public partial class DevolverFixturePage : ContentPage
{
    FixtureItem fixtureActual;

    bool fotoTomada = false;

    // Constructor cuando se abre desde el men� principal
    public DevolverFixturePage()
    {
        InitializeComponent();

        CargarFixturesEnUso();
    }
    async void VolverButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    // Constructor cuando se abre desde otra p�gina con una fixture espec�fica
    public DevolverFixturePage(FixtureItem fixture)
    {
        InitializeComponent();

        fixtureActual = fixture;

        CargarFixture();
    }

    void CargarFixturesEnUso()
    {
        // Simulaci�n temporal hasta conectar con API o BD

        fixtureActual = new FixtureItem
        {
            Serial = "FX-1002",
            Proceso = "Nutplates",
            Responsable = "Juan P�rez",
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
            var foto = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Tomar fotografía"
            });

            if (foto != null)
            {
                fotoTomada = true;

                await DisplayAlert(
                    "Fotografía capturada",
                    "La evidencia fue tomada correctamente.",
                    "OK");
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert(
                "No disponible",
                "Este dispositivo no soporta la cámara.",
                "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert(
                "Permiso denegado",
                "Se requiere permiso de cámara.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"No fue posible abrir la cámara: {ex.Message}",
                "OK");
        }
    }

    async void ConfirmarDevolucion(object sender, EventArgs e)
    {
        if (!fotoTomada)
        {
            await DisplayAlert(
                "Fotograf�a requerida",
                "Debe tomar una fotograf�a de la fixture antes de devolverla.",
                "OK");

            return;
        }

        string observaciones = ObservacionesEntry.Text;

        // Aqu� ir� la llamada a BD o API
        // Ejemplo futuro:
        // await fixtureService.DevolverFixture(fixtureActual.Serial, observaciones);

        await DisplayAlert(
            "Devoluci�n registrada",
            "La fixture fue devuelta correctamente.",
            "OK");

        await Navigation.PopAsync();
    }
}