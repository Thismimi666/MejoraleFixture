using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class DevolverFixturePage : ContentPage
{
    FixtureItem fixtureActual;
    readonly FixtureDatabase _db;
    bool fotoTomada = false;
    string fotoRuta = null;

    // Constructor cuando se abre desde el menú principal: carga fixtures del usuario logueado
    public DevolverFixturePage()
    {
        InitializeComponent();
        _db = IPlatformApplication.Current.Services.GetService<FixtureDatabase>();
        CargarFixturesEnUsoAsync();
    }

    async void VolverButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Constructor cuando se abre desde otra página con una fixture específica
    public DevolverFixturePage(FixtureItem fixture)
    {
        InitializeComponent();
        _db = IPlatformApplication.Current.Services.GetService<FixtureDatabase>();
        fixtureActual = fixture;
        CargarFixture();
    }

    async void CargarFixturesEnUsoAsync()
    {
        var usuario = SesionService.UsuarioActual;
        if (usuario == null) return;

        List<FixtureItem> lista;

        // Supervisores y admin ven todas las fixtures en uso
        if (usuario.EsSupervisor)
            lista = await _db.ObtenerTodasFixturesEnUsoAsync();
        else
            lista = await _db.ObtenerFixturesEnUsoDeUsuarioAsync(usuario.Nombre);

        if (lista.Count == 0)
        {
            await DisplayAlert("Sin fixtures", "No tienes fixtures en uso actualmente.", "OK");
            await Navigation.PopAsync();
            return;
        }

        // Si solo tiene una, la carga directo
        if (lista.Count == 1)
        {
            fixtureActual = lista[0];
            CargarFixture();
            return;
        }

        // Si tiene varias, muestra un selector
        var opciones = lista.Select(f => $"{f.Serial} — {f.Proceso}").ToArray();
        string seleccion = await DisplayActionSheet("¿Qué fixture deseas devolver?", "Cancelar", null, opciones);
        if (seleccion == null || seleccion == "Cancelar")
        {
            await Navigation.PopAsync();
            return;
        }

        fixtureActual = lista[Array.IndexOf(opciones, seleccion)];
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
                fotoRuta = await FixtureDatabase.GuardarFotoAsync(foto);
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
                "Fotografía requerida",
                "Debe tomar una fotografía de la fixture antes de devolverla.",
                "OK");
            return;
        }

        string observaciones = ObservacionesEntry?.Text;

        // Registrar movimiento de devolución
        await _db.RegistrarMovimientoAsync(new MovimientoFixture
        {
            FixtureSerial = fixtureActual.Serial,
            Tipo = "Devolución",
            Usuario = fixtureActual.Responsable,
            Observaciones = observaciones,
            FotoRuta = fotoRuta,
            Fecha = DateTime.Now
        });

        // Liberar fixture
        fixtureActual.Estado = "Disponible";
        fixtureActual.Responsable = "Sin asignar";
        await _db.GuardarAsync(fixtureActual);

        await DisplayAlert(
            "Devolución registrada",
            "La fixture fue devuelta correctamente.",
            "OK");

        await Navigation.PopAsync();
    }
}