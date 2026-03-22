using MejoraFixturesNortesV2.Models;
using SQLite;

namespace MejoraFixturesNortesV2.Data
{
    public class FixtureDatabase
    {
        SQLiteAsyncConnection _db;
        bool _inicializado = false;

        public FixtureDatabase()
        {
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "fixtures.db3");

            _db = new SQLiteAsyncConnection(dbPath,
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);
        }

        async Task InicializarAsync()
        {
            if (_inicializado) return;
            _inicializado = true;

            await _db.CreateTableAsync<FixtureItem>();
            await _db.CreateTableAsync<MovimientoFixture>();
            await _db.CreateTableAsync<Usuario>();

            // Seed Fixtures
            int countF = await _db.Table<FixtureItem>().CountAsync();
            if (countF == 0)
            {
                await _db.InsertAllAsync(new[]
                {
                    new FixtureItem { Serial = "FX-1001", Proceso = "Sellador",   Responsable = "Sin asignar", Estado = "Disponible", Imagen = "fixture1.png" },
                    new FixtureItem { Serial = "FX-1002", Proceso = "Nutplates",  Responsable = "Juan Pérez",  Estado = "En uso",     Imagen = "fixture2.png" },
                    new FixtureItem { Serial = "FX-1003", Proceso = "Torque",     Responsable = "Sin asignar", Estado = "Dañada",     Imagen = "fixture3.png" },
                    new FixtureItem { Serial = "FX-1004", Proceso = "Tierras",    Responsable = "Sin asignar", Estado = "Baja",       Imagen = "fixture4.png" },
                    new FixtureItem { Serial = "FX-1005", Proceso = "Remachado",  Responsable = "María López", Estado = "En uso",     Imagen = "fixture5.png" },
                    new FixtureItem { Serial = "FX-1006", Proceso = "Inspección", Responsable = "Sin asignar", Estado = "Disponible", Imagen = "fixture6.png" },
                });
            }

            // Seed Usuarios
            int countU = await _db.Table<Usuario>().CountAsync();
            if (countU == 0)
            {
                await _db.InsertAllAsync(new[]
                {
                    new Usuario { NumeroEmpleado = "EMP001", Nombre = "Juan Pérez",  Password = "1234", Nivel = 1 },
                    new Usuario { NumeroEmpleado = "EMP002", Nombre = "María López", Password = "1234", Nivel = 1 },
                    new Usuario { NumeroEmpleado = "Admin",  Nombre = "Administrador", Password = "admin123", Nivel = 9 },
                });
            }
        }

        public async Task<List<FixtureItem>> ObtenerTodosAsync()
        {
            await InicializarAsync();
            return await _db.Table<FixtureItem>().ToListAsync();
        }

        public async Task<FixtureItem> ObtenerPorSerialAsync(string serial)
        {
            await InicializarAsync();
            return await _db.Table<FixtureItem>()
                            .Where(f => f.Serial == serial)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> GuardarAsync(FixtureItem fixture)
        {
            await InicializarAsync();
            if (fixture.Id != 0)
                return await _db.UpdateAsync(fixture);
            return await _db.InsertAsync(fixture);
        }

        public async Task<int> EliminarAsync(FixtureItem fixture)
        {
            await InicializarAsync();
            return await _db.DeleteAsync(fixture);
        }

        public async Task ActualizarEstadoAsync(string serial, string nuevoEstado)
        {
            await InicializarAsync();
            var fixture = await ObtenerPorSerialAsync(serial);
            if (fixture != null)
            {
                fixture.Estado = nuevoEstado;
                await _db.UpdateAsync(fixture);
            }
        }

        // ── MOVIMIENTOS ────────────────────────────────────────────

        public async Task<int> RegistrarMovimientoAsync(MovimientoFixture movimiento)
        {
            await InicializarAsync();
            return await _db.InsertAsync(movimiento);
        }

        public async Task<List<MovimientoFixture>> ObtenerMovimientosPorSerialAsync(string serial)
        {
            await InicializarAsync();
            return await _db.Table<MovimientoFixture>()
                            .Where(m => m.FixtureSerial == serial)
                            .OrderByDescending(m => m.Fecha)
                            .ToListAsync();
        }

        public async Task<List<MovimientoFixture>> ObtenerTodosMovimientosAsync()
        {
            await InicializarAsync();
            return await _db.Table<MovimientoFixture>()
                            .OrderByDescending(m => m.Fecha)
                            .ToListAsync();
        }

        // ── FOTO ───────────────────────────────────────────────────

        /// <summary>
        /// Copia la foto capturada a AppDataDirectory/fotos/ y devuelve la ruta final.
        /// </summary>
        public static async Task<string> GuardarFotoAsync(FileResult foto)
        {
            string carpeta = Path.Combine(FileSystem.AppDataDirectory, "fotos");
            Directory.CreateDirectory(carpeta);

            string nombreArchivo = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(foto.FileName)}";
            string rutaDestino = Path.Combine(carpeta, nombreArchivo);

            using var origen = await foto.OpenReadAsync();
            using var destino = File.OpenWrite(rutaDestino);
            await origen.CopyToAsync(destino);

            return rutaDestino;
        }

        // ── USUARIOS ───────────────────────────────────────────────

        /// <summary>Valida credenciales. Devuelve el usuario o null si no coincide.</summary>
        public async Task<Usuario> AutenticarAsync(string numeroEmpleado, string password)
        {
            await InicializarAsync();
            return await _db.Table<Usuario>()
                            .Where(u => u.NumeroEmpleado == numeroEmpleado && u.Password == password)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            await InicializarAsync();
            return await _db.Table<Usuario>().ToListAsync();
        }

        public async Task<int> GuardarUsuarioAsync(Usuario usuario)
        {
            await InicializarAsync();
            if (usuario.Id != 0)
                return await _db.UpdateAsync(usuario);
            return await _db.InsertAsync(usuario);
        }

        /// <summary>Fixtures actualmente en uso por el nombre del responsable.</summary>
        public async Task<List<FixtureItem>> ObtenerFixturesEnUsoDeUsuarioAsync(string nombreUsuario)
        {
            await InicializarAsync();
            return await _db.Table<FixtureItem>()
                            .Where(f => f.Estado == "En uso" && f.Responsable == nombreUsuario)
                            .ToListAsync();
        }

        /// <summary>Todas las fixtures en uso (para supervisores/admin).</summary>
        public async Task<List<FixtureItem>> ObtenerTodasFixturesEnUsoAsync()
        {
            await InicializarAsync();
            return await _db.Table<FixtureItem>()
                            .Where(f => f.Estado == "En uso")
                            .ToListAsync();
        }
    }
}
