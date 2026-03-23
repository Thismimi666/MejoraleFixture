using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class DanosPage : ContentPage
{
    readonly FixtureDatabase _db = null!;
    string fotoPath = "";
    FileResult? fotoResult = null;

    public DanosPage()
    {
        InitializeComponent();
        _db = IPlatformApplication.Current!.Services.GetService<FixtureDatabase>()!;
    }

    // Permite abrir la página ya con un serial pre-cargado (desde PerfilPage)
    public DanosPage(string serial) : this()
    {
        SerialEntry.Text = serial;
    }

    private async void TomarFoto(object sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Error", "La cámara no está disponible", "OK");
                return;
            }

            var foto = await MediaPicker.Default.CapturePhotoAsync();

            if (foto != null)
            {
                fotoResult = foto;
                using var stream = await foto.OpenReadAsync();
                var ruta = Path.Combine(FileSystem.CacheDirectory, foto.FileName);
                using (var fileStream = File.OpenWrite(ruta))
                    await stream.CopyToAsync(fileStream);

                FotoPreview.Source = ImageSource.FromFile(ruta);
                fotoPath = ruta;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void EscanearCodigo(object sender, EventArgs e)
    {
        var resultado = await ScannerService.EscanearFixture();
        if (!string.IsNullOrEmpty(resultado))
            SerialEntry.Text = resultado;
    }

    private async void GuardarReporte(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SerialEntry.Text))
        {
            await DisplayAlert("Error", "Ingresa o escanea el serial", "OK");
            return;
        }

        if (string.IsNullOrEmpty(fotoPath))
        {
            await DisplayAlert("Error", "Toma una foto como evidencia", "OK");
            return;
        }

        var serial = SerialEntry.Text.Trim();

        var fixture = await _db.ObtenerPorSerialAsync(serial);
        if (fixture == null)
        {
            await DisplayAlert("Error", $"No se encontró la fixture '{serial}' en la base de datos.", "OK");
            return;
        }

        // Guardar foto en almacenamiento permanente
        string rutaFoto = fotoPath;
        if (fotoResult != null)
            rutaFoto = await FixtureDatabase.GuardarFotoAsync(fotoResult);

        // Registrar movimiento de daño
        await _db.RegistrarMovimientoAsync(new MovimientoFixture
        {
            FixtureSerial = serial,
            Tipo = "Daño",
            Usuario = Services.SesionService.UsuarioActual?.Nombre ?? "Desconocido",
            Observaciones = ComentarioEditor.Text?.Trim(),
            FotoRuta = rutaFoto,
            Fecha = DateTime.Now
        });

        // Marcar fixture como Dañada
        fixture.Estado = "Dañada";
        await _db.GuardarAsync(fixture);

        await DisplayAlert("Éxito", $"Fixture {serial} marcada como dañada y reporte guardado.", "OK");

        await Navigation.PopAsync();
    }
}