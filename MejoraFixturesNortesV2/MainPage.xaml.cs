using MejoraFixturesNortesV2.Views;

namespace MejoraFixturesNortesV2
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            CargarSistema();
        }

        private async void CargarSistema()
        {
            // Simulación de carga del sistema
            await Task.Delay(3000);

            await DisplayAlert("Sistema listo",
                               "SafeFix Control cargado correctamente.",
                               "Continuar");

            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}