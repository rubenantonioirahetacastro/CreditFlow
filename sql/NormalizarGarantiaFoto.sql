/*
 Ejecutar solamente después de desplegar la versión del API que obtiene
 tipo, marca, año y valor desde dbo.Garantia.
*/

IF COL_LENGTH('dbo.Garantia', 'nTipoGarantia') IS NULL
    THROW 50001, 'Primero debe ejecutar DireccionPersonaYGarantia.sql.', 1;
GO

IF COL_LENGTH('dbo.GarantiaFoto', 'IdArticuloGarantia') IS NOT NULL
   AND COL_LENGTH('dbo.GarantiaFoto', 'nValor') IS NOT NULL
BEGIN
    EXEC(N'
        WITH DatosGarantia AS
        (
            SELECT
                IdGarantia,
                IdArticuloGarantia,
                nValor,
                ROW_NUMBER() OVER (PARTITION BY IdGarantia ORDER BY IdFoto) AS NumeroFila
            FROM dbo.GarantiaFoto
        )
        UPDATE g
        SET
            g.nTipoGarantia = CASE
                WHEN g.nTipoGarantia = 0 THEN d.IdArticuloGarantia
                ELSE g.nTipoGarantia
            END,
            g.nValor = CASE
                WHEN g.nValor = 0 THEN d.nValor
                ELSE g.nValor
            END
        FROM dbo.Garantia AS g
        INNER JOIN DatosGarantia AS d
            ON d.IdGarantia = g.IdGarantia
           AND d.NumeroFila = 1;');
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nIdArticuloGarantia') IS NOT NULL
BEGIN
    EXEC(N'
        UPDATE dbo.Garantia
        SET nTipoGarantia = nIdArticuloGarantia
        WHERE nTipoGarantia = 0;');
END;
GO

IF EXISTS
(
    SELECT 1
    FROM dbo.GarantiaFoto AS gf
    LEFT JOIN dbo.Garantia AS g
        ON g.IdGarantia = gf.IdGarantia
    WHERE g.IdGarantia IS NULL
)
    THROW 50002, 'Existen fotografías con un IdGarantia inexistente. Corrija esos registros antes de crear la relación.', 1;
GO

IF COL_LENGTH('dbo.GarantiaFoto', 'IdArticuloGarantia') IS NOT NULL
BEGIN
    DECLARE @constraintArticulo SYSNAME;

    SELECT @constraintArticulo = dc.name
    FROM sys.default_constraints AS dc
    INNER JOIN sys.columns AS c
        ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.GarantiaFoto')
      AND c.name = 'IdArticuloGarantia';

    IF @constraintArticulo IS NOT NULL
        EXEC('ALTER TABLE dbo.GarantiaFoto DROP CONSTRAINT [' + @constraintArticulo + ']');

    ALTER TABLE dbo.GarantiaFoto DROP COLUMN IdArticuloGarantia;
END;
GO

IF COL_LENGTH('dbo.GarantiaFoto', 'nValor') IS NOT NULL
BEGIN
    DECLARE @constraintValor SYSNAME;

    SELECT @constraintValor = dc.name
    FROM sys.default_constraints AS dc
    INNER JOIN sys.columns AS c
        ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.GarantiaFoto')
      AND c.name = 'nValor';

    IF @constraintValor IS NOT NULL
        EXEC('ALTER TABLE dbo.GarantiaFoto DROP CONSTRAINT [' + @constraintValor + ']');

    ALTER TABLE dbo.GarantiaFoto DROP COLUMN nValor;
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nIdArticuloGarantia') IS NOT NULL
BEGIN
    DECLARE @constraintArticuloAnterior SYSNAME;

    SELECT @constraintArticuloAnterior = dc.name
    FROM sys.default_constraints AS dc
    INNER JOIN sys.columns AS c
        ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Garantia')
      AND c.name = 'nIdArticuloGarantia';

    IF @constraintArticuloAnterior IS NOT NULL
        EXEC('ALTER TABLE dbo.Garantia DROP CONSTRAINT [' + @constraintArticuloAnterior + ']');

    ALTER TABLE dbo.Garantia DROP COLUMN nIdArticuloGarantia;
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nIdFotoGarantia') IS NOT NULL
BEGIN
    DECLARE @constraintFotoAnterior SYSNAME;

    SELECT @constraintFotoAnterior = dc.name
    FROM sys.default_constraints AS dc
    INNER JOIN sys.columns AS c
        ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID('dbo.Garantia')
      AND c.name = 'nIdFotoGarantia';

    IF @constraintFotoAnterior IS NOT NULL
        EXEC('ALTER TABLE dbo.Garantia DROP CONSTRAINT [' + @constraintFotoAnterior + ']');

    ALTER TABLE dbo.Garantia DROP COLUMN nIdFotoGarantia;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_key_columns AS fkc
    WHERE fkc.parent_object_id = OBJECT_ID('dbo.GarantiaFoto')
      AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'IdGarantia'
      AND fkc.referenced_object_id = OBJECT_ID('dbo.Garantia')
      AND COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) = 'IdGarantia'
)
BEGIN
    ALTER TABLE dbo.GarantiaFoto WITH CHECK
        ADD CONSTRAINT FK_GarantiaFoto_Garantia
        FOREIGN KEY (IdGarantia)
        REFERENCES dbo.Garantia (IdGarantia);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.GarantiaFoto')
      AND name = 'IX_GarantiaFoto_IdGarantia'
)
BEGIN
    CREATE INDEX IX_GarantiaFoto_IdGarantia
        ON dbo.GarantiaFoto (IdGarantia);
END;
GO
