using MejoraFixturesNortesV2.Data;
using MejoraFixturesNortesV2.Models;

namespace MejoraFixturesNortesV2.Services;

public class AuditoriaService
{
    readonly FixtureDatabase _db = null!;

    public static event Action? EventosActualizados;

    public AuditoriaService()
    {
        _db = IPlatformApplication.Current!.Services.GetService<FixtureDatabase>()!;
    }

    public async Task<List<EventoAuditoria>> ObtenerEventosAsync()
        => await _db.ObtenerEventosAsync();

    public async Task<List<EventoAuditoria>> ObtenerEventosPorFechaAsync(DateTime fecha)
        => await _db.ObtenerEventosPorFechaAsync(fecha);

    public async Task CrearEventoAsync(EventoAuditoria evento)
    {
        await _db.GuardarEventoAsync(evento);
        EventosActualizados?.Invoke();
    }

    public async Task ActualizarEventoAsync(EventoAuditoria evento)
    {
        await _db.GuardarEventoAsync(evento);
        EventosActualizados?.Invoke();
    }

    public async Task EliminarEventoAsync(int id)
    {
        await _db.EliminarEventoAsync(id);
        EventosActualizados?.Invoke();
    }

}