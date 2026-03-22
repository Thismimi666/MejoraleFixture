using MejoraFixturesNortesV2.Models;
using MejoraFixturesNortesV2.Services;
using System.Globalization;

namespace MejoraFixturesNortesV2.Views;

public partial class AuditoriasPage : ContentPage
{
    DateTime mesActual = DateTime.Today;

    AuditoriaService auditoriaService = new AuditoriaService();

    List<EventoAuditoria> eventos = new();

    public AuditoriasPage()
    {
        InitializeComponent();

        eventos = auditoriaService.ObtenerEventos();

        // 🔥 SUSCRIPCIÓN AL EVENTO GLOBAL
        AuditoriaService.EventosActualizados += RecargarEventos;

        GenerarCalendario();
    }

    // 🔥 MÉTODO PARA RECARGAR AUTOMÁTICAMENTE
    void RecargarEventos()
    {
        eventos = auditoriaService.ObtenerEventos();
        GenerarCalendario();
    }

    void GenerarCalendario()
    {
        CalendarioGrid.Children.Clear();
        CalendarioGrid.RowDefinitions.Clear();
        CalendarioGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < 7; i++)
            CalendarioGrid.ColumnDefinitions.Add(new ColumnDefinition());

        for (int i = 0; i < 6; i++)
            CalendarioGrid.RowDefinitions.Add(new RowDefinition());

        MesLabel.Text =
            mesActual.ToString("MMMM yyyy", new CultureInfo("es-ES"));

        DateTime primerDia =
            new DateTime(mesActual.Year, mesActual.Month, 1);

        int inicio = (int)primerDia.DayOfWeek;

        int diasMes =
            DateTime.DaysInMonth(mesActual.Year, mesActual.Month);

        int fila = 0;
        int columna = inicio;

        for (int dia = 1; dia <= diasMes; dia++)
        {
            DateTime fecha =
                new DateTime(mesActual.Year, mesActual.Month, dia);

            Frame celda = new Frame
            {
                CornerRadius = 20,
                HeightRequest = 40,
                WidthRequest = 40,
                Padding = 5,
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.LightGray,
                Content = new Label
                {
                    Text = dia.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            var evento = eventos
                .FirstOrDefault(e => e.Fecha.Date == fecha.Date);

            if (evento != null)
            {
                if (evento.Tipo == "Interna")
                    celda.BackgroundColor = Color.FromArgb("#1F4E8C");

                if (evento.Tipo == "Externa")
                    celda.BackgroundColor = Color.FromArgb("#F9A825");

                if (evento.Tipo == "Evento")
                    celda.BackgroundColor = Color.FromArgb("#BDBDBD");
            }

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => MostrarEventos(fecha);
            celda.GestureRecognizers.Add(tap);

            CalendarioGrid.Add(celda, columna, fila);

            columna++;

            if (columna > 6)
            {
                columna = 0;
                fila++;
            }
        }
    }

    void MostrarEventos(DateTime fecha)
    {
        EventosContainer.Children.Clear();

        FechaSeleccionadaLabel.Text =
            fecha.ToString("dddd dd MMMM yyyy", new CultureInfo("es-ES"));

        var eventosDia = auditoriaService.ObtenerEventosPorFecha(fecha);

        if (eventosDia.Count == 0)
        {
            EventosContainer.Children.Add(new Frame
            {
                BackgroundColor = Color.FromArgb("#F4F6F8"),
                CornerRadius = 12,
                Padding = 15,
                Content = new Label
                {
                    Text = "No hay eventos programados",
                    TextColor = Colors.Gray,
                    HorizontalOptions = LayoutOptions.Center
                }
            });

            return;
        }

        foreach (var ev in eventosDia)
        {
            Color color = Color.FromArgb("#BDBDBD");

            if (ev.Tipo == "Interna")
                color = Color.FromArgb("#1F4E8C");

            if (ev.Tipo == "Externa")
                color = Color.FromArgb("#F9A825");

            if (ev.Tipo == "Evento")
                color = Color.FromArgb("#6C757D");

            Frame card = new Frame
            {
                CornerRadius = 16,
                Padding = 0,
                HasShadow = true,
                BackgroundColor = Colors.White,
                Margin = new Thickness(0, 5),

                Content = new Grid
                {
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 6 },
                    new ColumnDefinition { Width = GridLength.Star }
                },

                    Children =
                {
                    // 🔵 Barra lateral de color
                    new BoxView
                    {
                        BackgroundColor = color
                    },

                    // 📄 Contenido
                    new VerticalStackLayout
                    {
                        Padding = 12,
                        Spacing = 5,
                        Children =
                        {
                            new Label
                            {
                                Text = ev.Titulo,
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 14,
                                TextColor = Color.FromArgb("#0D3B66")
                            },

                            new Label
                            {
                                Text = ev.Tipo,
                                FontSize = 12,
                                TextColor = color
                            },

                            new Label
                            {
                                Text = ev.Descripcion,
                                FontSize = 12,
                                TextColor = Colors.Gray
                            },

                            // 🔥 BOTÓN ELIMINAR
                            new Button
                            {
                                Text = "Eliminar",
                                BackgroundColor = Colors.Red,
                                TextColor = Colors.White,
                                FontSize = 12,
                                CornerRadius = 10,
                                HorizontalOptions = LayoutOptions.End,
                                Command = new Command(() =>
                                {
                                    auditoriaService.EliminarEvento(ev.Id);
                                })
                            }
                        }
                    }
                }
                }
            };

            EventosContainer.Children.Add(card);
        }
    }

    async void MesAnterior(object sender, EventArgs e)
    {
        await AnimarCambioMes(-1);
    }

    async void MesSiguiente(object sender, EventArgs e)
    {
        await AnimarCambioMes(1);
    }

    async Task AnimarCambioMes(int direccion)
    {
        int ancho = (int)this.Width;

        await Task.WhenAll(
            CalendarioGrid.FadeTo(0, 150),
            CalendarioGrid.TranslateTo(direccion == 1 ? -ancho : ancho, 0, 150)
        );

        mesActual = mesActual.AddMonths(direccion);

        GenerarCalendario();

        CalendarioGrid.TranslationX = direccion == 1 ? ancho : -ancho;

        await Task.WhenAll(
            CalendarioGrid.FadeTo(1, 150),
            CalendarioGrid.TranslateTo(0, 0, 150)
        );
    }

    // 🔥 LIMPIEZA DE EVENTO
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        AuditoriaService.EventosActualizados -= RecargarEventos;
    }
}