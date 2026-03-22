using Microsoft.Maui.Devices;
using Microsoft.Maui.Media;

namespace MejoraFixturesNortesV2.Services;

public class ScannerService
{
    public static async Task<string> EscanearFixture()
    {
        // 💻 WINDOWS → SOLO BLOQUEAR
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Escáner no disponible",
                "El escaneo solo está disponible en dispositivos Android.",
                "OK");

            return null;
        }

        // 📱 ANDROID → USAR CÁMARA
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "La cámara no está disponible",
                        "OK");

                    return null;
                }

                var foto = await MediaPicker.CapturePhotoAsync();

                if (foto == null)
                    return null;

                // 🔥 AQUÍ puedes después procesar la imagen (OCR o escáner real)

                await Application.Current.MainPage.DisplayAlert(
                    "Captura realizada",
                    "La imagen del código fue capturada correctamente.",
                    "OK");

                return null; // aún no hay lectura automática
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                return null;
            }
        }

        return null;
    }
}