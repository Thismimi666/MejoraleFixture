using MejoraFixturesNortesV2.Views;

namespace MejoraFixturesNortesV2.Controls;

public partial class BottomMenu : ContentView
{
    public BottomMenu()
    {
        InitializeComponent();
    }

    private async void OnCatalogoTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new CatalogoFixturesPage());
    }

    private async void OnAuditoriasTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new AuditoriasPage());
    }

    private async void OnReportesTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.DisplayAlert("Reportes", "En desarrollo", "OK");
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new PerfilPage());
    }
}