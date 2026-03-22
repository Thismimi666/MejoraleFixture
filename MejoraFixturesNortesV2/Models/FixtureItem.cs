using SQLite;

namespace MejoraFixturesNortesV2.Models
{
    [Table("Fixtures")]
    public class FixtureItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Serial { get; set; }

        [NotNull]
        public string Proceso { get; set; }

        public string Responsable { get; set; } = "Sin asignar";

        public string Estado { get; set; } = "Disponible";

        public string Imagen { get; set; }

        [Ignore]
        public bool PuedeSolicitar =>
            Estado != "En uso" &&
            Estado != "Dañada" &&
            Estado != "Baja";
    }
}