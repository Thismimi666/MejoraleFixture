using MejoraFixturesNortesV2.Services;
using Microsoft.Maui.Media;
using System.IO;

namespace MejoraFixturesNortesV2.Views;

public partial class DanosPage : ContentPage
{
    string fotoPath = "";

    public DanosPage()
    {
        InitializeComponent();
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

            var foto = await MediaPicker.CapturePhotoAsync();

            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();

                var ruta = Path.Combine(FileSystem.CacheDirectory, foto.FileName);

                using (var fileStream = File.OpenWrite(ruta))
                {
                    await stream.CopyToAsync(fileStream);
                }

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
        {
            SerialEntry.Text = resultado;
        }
    }

    private async void GuardarReporte(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SerialEntry.Text))
        {
            await DisplayAlert("Error", "Ingresa o escanea el serial", "OK");
            return;
        }

        if (string.IsNullOrEmpty(fotoPath))
        {
            await DisplayAlert("Error", "Toma una foto como evidencia", "OK");
            return;
        }

        string comentario = ComentarioEditor.Text;

        await DisplayAlert("Éxito", "Reporte guardado correctamente", "OK");

        await Navigation.PopAsync();
    }
}