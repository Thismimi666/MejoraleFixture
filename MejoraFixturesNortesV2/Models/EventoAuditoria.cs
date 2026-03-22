namespace MejoraFixturesNortesV2.Models;

public class EventoAuditoria
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    // Interna / Externa / Evento
    public string Tipo { get; set; }

    public string Titulo { get; set; }

    public string Descripcion { get; set; }
}