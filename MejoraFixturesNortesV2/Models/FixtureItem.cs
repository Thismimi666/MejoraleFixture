namespace MejoraFixturesNortesV2.Models
{
    public class FixtureItem
    {
        public string Serial { get; set; }

        public string Proceso { get; set; }

        public string Responsable { get; set; }

        public string Estado { get; set; }

        public string Imagen { get; set; }

        // Determina si el fixture puede solicitarse
        public bool PuedeSolicitar
        {
            get
            {
                return Estado != "En uso" &&
                       Estado != "Dañada" &&
                       Estado != "Baja";
            }
        }
    }
}