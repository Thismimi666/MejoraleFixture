using SQLite;

namespace MejoraFixturesNortesV2.Models;

[Table("Eventos")]
public class EventoAuditoria
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    // Interna / Externa / Evento
    public string Tipo { get; set; }

    public string Titulo { get; set; }

    public string Descripcion { get; set; }
}