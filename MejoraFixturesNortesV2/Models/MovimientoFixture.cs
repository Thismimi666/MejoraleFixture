using SQLite;

namespace MejoraFixturesNortesV2.Models
{
    /// <summary>
    /// Registra cada solicitud o devolución de un fixture.
    /// Tipo: "Solicitud" | "Devolución"
    /// FotoRuta: ruta absoluta al archivo jpg guardado en AppDataDirectory/fotos/
    /// </summary>
    [Table("Movimientos")]
    public class MovimientoFixture
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>FK → Fixtures.Serial</summary>
        [Indexed, NotNull]
        public string FixtureSerial { get; set; }

        /// <summary>"Solicitud" o "Devolución"</summary>
        [NotNull]
        public string Tipo { get; set; }

        [NotNull]
        public string Usuario { get; set; }

        public string Observaciones { get; set; }

        /// <summary>Ruta local al archivo de foto (.jpg)</summary>
        public string FotoRuta { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
