using MejoraFixturesNortesV2.Services;
namespace MejoraFixturesNortesV2.Views;

public partial class MenuPrincipalPage : ContentPage
{
    public MenuPrincipalPage()
    {
        InitializeComponent();
    }

    private async void AbrirCatalogo(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatalogoFixturesPage());
    }

    private async void AbrirCalendario(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AuditoriasPage());
    }

    private async void AbrirDevolucion(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DevolverFixturePage());
    }

    private void CerrarSesion(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
    private async void AbrirEscaner(object sender, EventArgs e)
    {
        await ScannerService.EscanearFixture();
    }
}