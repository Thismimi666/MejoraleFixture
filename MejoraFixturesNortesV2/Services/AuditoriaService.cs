using System.Linq;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Services;

public class AuditoriaService
{
    // 🔥 LISTA GLOBAL COMPARTIDA
    private static List<EventoAuditoria> eventos = new();

    // 🔥 EVENTO PARA ACTUALIZAR UI
    public static event Action EventosActualizados;

    public AuditoriaService()
    {
        // Evita duplicar datos demo
        if (eventos.Count == 0)
            CargarEventosDemo();
    }

    void CargarEventosDemo()
    {
        eventos.Add(new EventoAuditoria
        {
            Id = 1,
            Fecha = new DateTime(2024, 5, 7),
            Tipo = "Externa",
            Titulo = "Copa de Calidad",
            Descripcion = "Auditoría externa"
        });

        eventos.Add(new EventoAuditoria
        {
            Id = 2,
            Fecha = new DateTime(2024, 5, 13),
            Tipo = "Interna",
            Titulo = "Auditoría Producción",
            Descripcion = "Auditoría interna"
        });

        eventos.Add(new EventoAuditoria
        {
            Id = 3,
            Fecha = new DateTime(2024, 5, 21),
            Tipo = "Evento",
            Titulo = "Delegación cliente",
            Descripcion = "Visita técnica"
        });
    }

    // ✅ READ (todos)
    public List<EventoAuditoria> ObtenerEventos()
    {
        return eventos;
    }

    // ✅ READ por fecha
    public List<EventoAuditoria> ObtenerEventosPorFecha(DateTime fecha)
    {
        return eventos
            .Where(e => e.Fecha.Date == fecha.Date)
            .ToList();
    }

    // ✅ CREATE
    public void CrearEvento(EventoAuditoria evento)
    {
        evento.Id = eventos.Count > 0 ? eventos.Max(e => e.Id) + 1 : 1;

        eventos.Add(evento);

        EventosActualizados?.Invoke(); // 🔥 refresca UI
    }

    // ✅ UPDATE
    public void ActualizarEvento(EventoAuditoria evento)
    {
        var existente = eventos.FirstOrDefault(e => e.Id == evento.Id);

        if (existente != null)
        {
            existente.Fecha = evento.Fecha;
            existente.Tipo = evento.Tipo;
            existente.Titulo = evento.Titulo;
            existente.Descripcion = evento.Descripcion;

            EventosActualizados?.Invoke(); // 🔥 refresca UI
        }
    }

    // ✅ DELETE
    public void EliminarEvento(int id)
    {
        var evento = eventos.FirstOrDefault(e => e.Id == id);

        if (evento != null)
        {
            eventos.Remove(evento);

            EventosActualizados?.Invoke(); // 🔥 refresca UI
        }
    }
}