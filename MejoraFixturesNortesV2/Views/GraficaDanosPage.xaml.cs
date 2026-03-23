using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;
using System.Globalization;

namespace MejoraFixturesNortesV2.Views;

public partial class GraficaDanosPage : ContentPage
{
    readonly FixtureDatabase _db = null!;
    List<MovimientoFixture> _todos = new();
    string _filtro = "Daños"; // Daños | Bajas | Daños+Bajas

    public GraficaDanosPage()
    {
        InitializeComponent();
        _db = IPlatformApplication.Current!.Services.GetService<FixtureDatabase>()!;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _todos = await _db.ObtenerTodosMovimientosAsync();
        ActualizarChips();
        DibujarGrafica();
    }

    void FiltroGrafica_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn == BtnGDanos) _filtro = "Daños";
        else if (btn == BtnGBajas) _filtro = "Bajas";
        else _filtro = "Daños+Bajas";

        ActualizarChips();
        DibujarGrafica();
    }

    void ActualizarChips()
    {
        var inactivo = Color.FromArgb("#E0E0E0");
        var textoInactivo = Color.FromArgb("#444444");

        BtnGDanos.BackgroundColor = inactivo; BtnGDanos.TextColor = textoInactivo;
        BtnGBajas.BackgroundColor = inactivo; BtnGBajas.TextColor = textoInactivo;
        BtnGAmbos.BackgroundColor = inactivo; BtnGAmbos.TextColor = textoInactivo;

        switch (_filtro)
        {
            case "Daños":
                BtnGDanos.BackgroundColor = Color.FromArgb("#C62828");
                BtnGDanos.TextColor = Colors.White;
                break;
            case "Bajas":
                BtnGBajas.BackgroundColor = Color.FromArgb("#4A148C");
                BtnGBajas.TextColor = Colors.White;
                break;
            default:
                BtnGAmbos.BackgroundColor = Color.FromArgb("#0D3B66");
                BtnGAmbos.TextColor = Colors.White;
                break;
        }
    }

    void DibujarGrafica()
    {
        var filtrados = FiltrarPorTipo(_todos);

        // Resumen
        var ahora = DateTime.Today;
        LblTotalMes.Text = filtrados.Count(m => m.Fecha.Year == ahora.Year && m.Fecha.Month == ahora.Month).ToString();
        LblTotalAnio.Text = filtrados.Count(m => m.Fecha.Year == ahora.Year).ToString();

        // Agrupar por mes (últimos 12)
        var datosMes = Enumerable.Range(0, 12)
            .Select(i => ahora.AddMonths(-11 + i))
            .Select(mes => new
            {
                Etiqueta = mes.ToString("MMM yy", new CultureInfo("es-ES")),
                Count = filtrados.Count(m => m.Fecha.Year == mes.Year && m.Fecha.Month == mes.Month)
            })
            .ToList();

        int maxVal = datosMes.Max(d => d.Count);
        double maxBarWidth = 220;

        BarsContainer.Children.Clear();

        // Título
        BarsContainer.Children.Add(new Label
        {
            Text = $"Últimos 12 meses  —  {EtiquetaFiltro()}",
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            TextColor = Color.FromArgb("#0D3B66"),
            Margin = new Thickness(0, 0, 0, 8)
        });

        foreach (var d in datosMes)
        {
            double barWidth = maxVal > 0 ? d.Count / (double)maxVal * maxBarWidth : 0;
            Color barColor = _filtro == "Daños" ? Color.FromArgb("#C62828")
                           : _filtro == "Bajas" ? Color.FromArgb("#4A148C")
                           : Color.FromArgb("#0D3B66");

            var mesLabel = new Label
            {
                Text = char.ToUpper(d.Etiqueta[0]) + d.Etiqueta[1..],
                FontSize = 13,
                TextColor = Color.FromArgb("#555"),
                WidthRequest = 68,
                VerticalOptions = LayoutOptions.Center
            };

            var barra = new BoxView
            {
                Color = d.Count > 0 ? barColor : Color.FromArgb("#E0E0E0"),
                HeightRequest = 26,
                WidthRequest = Math.Max(barWidth, d.Count > 0 ? 6 : 3),
                CornerRadius = 5,
                VerticalOptions = LayoutOptions.Center
            };

            var cuentaLabel = new Label
            {
                Text = d.Count.ToString(),
                FontSize = 14,
                FontAttributes = d.Count > 0 ? FontAttributes.Bold : FontAttributes.None,
                TextColor = d.Count > 0 ? barColor : Color.FromArgb("#AAAAAA"),
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(8, 0, 0, 0)
            };

            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 70 },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 0
            };
            Grid.SetColumn(mesLabel, 0);
            Grid.SetColumn(barra, 1);
            Grid.SetColumn(cuentaLabel, 2);
            row.Add(mesLabel);
            row.Add(barra);
            row.Add(cuentaLabel);

            BarsContainer.Children.Add(row);
        }
    }

    IEnumerable<MovimientoFixture> FiltrarPorTipo(IEnumerable<MovimientoFixture> lista) => _filtro switch
    {
        "Daños" => lista.Where(m => m.Tipo == "Daño"),
        "Bajas" => lista.Where(m => m.Tipo == "Pérdida"),
        _ => lista.Where(m => m.Tipo == "Daño" || m.Tipo == "Pérdida")
    };

    string EtiquetaFiltro() => _filtro switch
    {
        "Daños" => "Daños",
        "Bajas" => "Bajas / Pérdidas",
        _ => "Daños + Bajas"
    };

    async void VolverButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
