using SQLite;

namespace MejoraFixturesNortesV2.Models
{
    /// <summary>
    /// Niveles:
    ///   1 = Operador  — puede solicitar y devolver sus propias fixtures
    ///   2 = Supervisor — igual que operador pero ve todas las fixtures en uso
    ///   9 = Admin     — acceso completo
    /// </summary>
    [Table("Usuarios")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string NumeroEmpleado { get; set; }

        [NotNull]
        public string Nombre { get; set; }

        [NotNull]
        public string Password { get; set; }

        /// <summary>1=Operador, 2=Supervisor, 9=Admin</summary>
        public int Nivel { get; set; } = 1;

        [Ignore]
        public bool EsAdmin => Nivel == 9;

        [Ignore]
        public bool EsSupervisor => Nivel >= 2;
    }
}
