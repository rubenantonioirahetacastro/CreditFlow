-- Permisos de menú por rol (mantenimiento «Permisos por rol» de la Web).
-- Idempotente: crea la tabla solo si no existe y carga los permisos iniciales solo si está vacía.
-- Los permisos iniciales reproducen el acceso que había antes de este mantenimiento:
--   * Todos los roles: Inicio y las opciones de Otorgamiento.
--   * Simulador: Admin (1), Supervisor (3), Oficial de crédito (4) y Tecnología (7).
--   * Mantenimientos: ver para 1, 3 y 7; crear/editar/eliminar para 1 y 7 (Roles y Catálogos también 3).
--   * Permisos por rol: 1 y 7 (además la API y la Web siempre se lo garantizan a esos dos roles).

IF OBJECT_ID(N'dbo.RolMenuPermiso', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RolMenuPermiso
    (
        idRolMenuPermiso   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RolMenuPermiso PRIMARY KEY,
        idRol              INT               NOT NULL,
        cClaveMenu         VARCHAR(100)      NOT NULL,
        bVer               BIT               NOT NULL CONSTRAINT DF_RolMenuPermiso_bVer DEFAULT (0),
        bCrear             BIT               NOT NULL CONSTRAINT DF_RolMenuPermiso_bCrear DEFAULT (0),
        bEditar            BIT               NOT NULL CONSTRAINT DF_RolMenuPermiso_bEditar DEFAULT (0),
        bEliminar          BIT               NOT NULL CONSTRAINT DF_RolMenuPermiso_bEliminar DEFAULT (0),
        dFechaModificacion DATETIME          NOT NULL CONSTRAINT DF_RolMenuPermiso_dFechaModificacion DEFAULT (GETDATE()),
        CONSTRAINT FK_RolMenuPermiso_Roles FOREIGN KEY (idRol) REFERENCES dbo.Roles (IdRol) ON DELETE CASCADE,
        CONSTRAINT UQ_RolMenuPermiso_Rol_Clave UNIQUE (idRol, cClaveMenu)
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.RolMenuPermiso)
BEGIN
    -- clave, roles que la ven, roles con crear/editar/eliminar ('' = ninguno). Listas de IDs entre comas.
    DECLARE @Inicial TABLE (cClaveMenu VARCHAR(100), cVer VARCHAR(50), cCrud VARCHAR(50));
    INSERT INTO @Inicial (cClaveMenu, cVer, cCrud) VALUES
        ('home',                                        '*',      ''),
        ('simulador',                                   ',1,3,4,7,', ''),
        ('otorgamiento.procesamiento.verificacion',     '*',      ''),
        ('otorgamiento.procesamiento.readecuacion',     '*',      ''),
        ('otorgamiento.autorizacion.autorizar-credito', '*',      ''),
        ('otorgamiento.tesoreria.abonar',               '*',      ''),
        ('otorgamiento.tesoreria.abonar-agencia',       '*',      ''),
        ('otorgamiento.agencia.registrar-oficiales',    '*',      ''),
        ('otorgamiento.agencia.abono-oficial',          '*',      ''),
        ('otorgamiento.desembolso.bandeja',             '*',      ''),
        ('mantenimientos.empleados',                    ',1,3,7,', ',1,7,'),
        ('mantenimientos.roles',                        ',1,3,7,', ',1,3,7,'),
        ('mantenimientos.permisos',                     ',1,7,',   ',1,7,'),
        ('mantenimientos.agencias',                     ',1,3,7,', ',1,7,'),
        ('mantenimientos.catalogos',                    ',1,3,7,', ',1,3,7,'),
        ('mantenimientos.lineas',                       ',1,3,7,', ',1,7,');

    INSERT INTO dbo.RolMenuPermiso (idRol, cClaveMenu, bVer, bCrear, bEditar, bEliminar)
    SELECT r.IdRol,
           i.cClaveMenu,
           1,
           CASE WHEN CHARINDEX(',' + CAST(r.IdRol AS VARCHAR(10)) + ',', i.cCrud) > 0 THEN 1 ELSE 0 END,
           CASE WHEN CHARINDEX(',' + CAST(r.IdRol AS VARCHAR(10)) + ',', i.cCrud) > 0 THEN 1 ELSE 0 END,
           CASE WHEN CHARINDEX(',' + CAST(r.IdRol AS VARCHAR(10)) + ',', i.cCrud) > 0 THEN 1 ELSE 0 END
    FROM dbo.Roles r
    CROSS JOIN @Inicial i
    WHERE i.cVer = '*' OR CHARINDEX(',' + CAST(r.IdRol AS VARCHAR(10)) + ',', i.cVer) > 0;
END;
