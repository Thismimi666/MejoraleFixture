using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;

namespace MejoraFixturesNortesV2.Views;

public partial class AgregarFixturePage : ContentPage
{
    string estadoSeleccionado = "Disponible";
    string rutaImagen = "";

    public AgregarFixturePage()
    {
        InitializeComponent();
    }

    private async void TomarFoto(object sender, EventArgs e)
    {
        try
        {
            var foto = await MediaPicker.CapturePhotoAsync();
            if (foto != null)
            {
                rutaImagen = foto.FullPath;
                imgFixture.Source = ImageSource.FromFile(rutaImagen);
            }
        }
        catch
        {
            await DisplayAlert("Error", "No se pudo abrir la cámara", "OK");
        }
    }

    private async void AbrirGaleria(object sender, EventArgs e)
    {
        try
        {
            var foto = await MediaPicker.PickPhotoAsync();
            if (foto != null)
            {
                rutaImagen = foto.FullPath;
                imgFixture.Source = ImageSource.FromFile(rutaImagen);
            }
        }
        catch
        {
            await DisplayAlert("Error", "No se pudo abrir la galería", "OK");
        }
    }

    private async void AbrirCatalogo(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatalogoFixturesPage(true));
    }

    // 🎯 Estados
    private void EstadoDisponible(object sender, EventArgs e) => estadoSeleccionado = "Disponible";
    private void EstadoEnUso(object sender, EventArgs e) => estadoSeleccionado = "En uso";
    private void EstadoDanada(object sender, EventArgs e) => estadoSeleccionado = "Dañada";
    private void EstadoBaja(object sender, EventArgs e) => estadoSeleccionado = "Baja";

    // 💾 Guardar con validaciones
    private async void GuardarFixture(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
            string.IsNullOrWhiteSpace(txtProceso.Text))
        {
            await DisplayAlert("Error", "Serial y Proceso son obligatorios", "OK");
            return;
        }

        // Validación PRO 🔥
        if (estadoSeleccionado == "En uso" && string.IsNullOrWhiteSpace(txtResponsable.Text))
        {
            await DisplayAlert("Error", "Debes asignar un responsable si está en uso", "OK");
            return;
        }

        var nuevo = new FixtureItem
        {
            Serial = txtCodigo.Text,
            Proceso = txtProceso.Text,
            Responsable = string.IsNullOrWhiteSpace(txtResponsable.Text) ? "Sin asignar" : txtResponsable.Text,
            Estado = estadoSeleccionado,
            Imagen = string.IsNullOrEmpty(rutaImagen) ? "fotofix.png" : rutaImagen
        };

        DataService.Fixtures.Add(nuevo);

        await DisplayAlert("Éxito", "Fixture agregado correctamente", "OK");

        await Navigation.PopAsync();
    }
}