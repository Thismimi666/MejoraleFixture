-- =====================================================
-- Script de referencia: Fixtures + Movimientos (SQLite)
-- =====================================================

-- Tabla principal de fixtures
CREATE TABLE IF NOT EXISTS Fixtures (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Serial      TEXT NOT NULL UNIQUE,
    Proceso     TEXT NOT NULL,
    Responsable TEXT NOT NULL DEFAULT 'Sin asignar',
    Estado      TEXT NOT NULL DEFAULT 'Disponible',
    Imagen      TEXT
);

-- Tabla de movimientos (solicitudes y devoluciones)
-- FotoRuta: ruta al .jpg guardado en AppDataDirectory/fotos/
CREATE TABLE IF NOT EXISTS Movimientos (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    FixtureSerial  TEXT NOT NULL REFERENCES Fixtures(Serial),
    Tipo           TEXT NOT NULL,
    Usuario        TEXT NOT NULL,
    Observaciones  TEXT,
    FotoRuta       TEXT,
    Fecha          TEXT NOT NULL
);

-- 6 fixtures de prueba
INSERT INTO Fixtures (Serial, Proceso, Responsable, Estado, Imagen) VALUES
    ('FX-1001', 'Sellador',   'Sin asignar', 'Disponible', 'fixture1.png'),
    ('FX-1002', 'Nutplates',  'Juan Perez',  'En uso',     'fixture2.png'),
    ('FX-1003', 'Torque',     'Sin asignar', 'Danada',     'fixture3.png'),
    ('FX-1004', 'Tierras',    'Sin asignar', 'Baja',       'fixture4.png'),
    ('FX-1005', 'Remachado',  'Maria Lopez', 'En uso',     'fixture5.png'),
    ('FX-1006', 'Inspeccion', 'Sin asignar', 'Disponible', 'fixture6.png');

-- Movimientos de ejemplo
INSERT INTO Movimientos (FixtureSerial, Tipo, Usuario, Observaciones, FotoRuta, Fecha) VALUES
    ('FX-1002', 'Solicitud',  'Juan Perez',  NULL,          '/fotos/20260320_foto.jpg', '2026-03-20T08:15:00'),
    ('FX-1005', 'Solicitud',  'Maria Lopez', NULL,          '/fotos/20260321_foto.jpg', '2026-03-21T09:00:00'),
    ('FX-1001', 'Solicitud',  'Carlos Ruiz', NULL,          '/fotos/20260322a.jpg',     '2026-03-22T07:30:00'),
    ('FX-1001', 'Devolucion', 'Carlos Ruiz', 'Sin novedad', '/fotos/20260322b.jpg',     '2026-03-22T12:00:00');

-- Ver historial de una fixture:
--   SELECT * FROM Movimientos WHERE FixtureSerial = 'FX-1002' ORDER BY Fecha DESC;
-- Ver quien tiene cada fixture en uso:
--   SELECT f.Serial, f.Responsable, m.Fecha FROM Fixtures f
--   JOIN Movimientos m ON m.FixtureSerial = f.Serial AND m.Tipo = 'Solicitud'
--   WHERE f.Estado = 'En uso'
--   AND m.Id = (SELECT MAX(Id) FROM Movimientos WHERE FixtureSerial = f.Serial);
