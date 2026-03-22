namespace MejoraFixturesNortesV2.Views;

public partial class AdminPage : ContentPage
{
    public AdminPage()
    {
        InitializeComponent();
    }

    private async void AbrirAgregar(object sender, EventArgs e)
    {
         await Navigation.PushAsync(new ModuloAdminPage());
    }

    private async void AbrirIncidencia(object sender, EventArgs e)
    {
        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("IncidenciaReporte.docx");

            var filePath = Path.Combine(FileSystem.CacheDirectory, "IncidenciaReporte.docx");

            using (var fileStream = File.Create(filePath))
            {
                await stream.CopyToAsync(fileStream);
            }

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo abrir el archivo: {ex.Message}", "OK");
        }
    }

    private async void AbrirNotas(object sender, EventArgs e)
    {
         await Navigation.PushAsync(new NotasAdminPage());
    }

    private async void AbrirHistorial(object sender, EventArgs e)
    {
        await DisplayAlert("Historial", "Módulo en desarrollo", "OK");
        // await Navigation.PushAsync(new HistorialPage());
    }

    private void CerrarSesion(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}