IF COL_LENGTH('dbo.Persona', 'cDireccion') IS NULL
BEGIN
    ALTER TABLE dbo.Persona
        ADD cDireccion VARCHAR(150) NOT NULL
            CONSTRAINT DF_Persona_cDireccion DEFAULT ('');
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nTipoGarantia') IS NULL
BEGIN
    ALTER TABLE dbo.Garantia
        ADD nTipoGarantia INT NOT NULL
            CONSTRAINT DF_Garantia_nTipoGarantia DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nMarca') IS NULL
BEGIN
    ALTER TABLE dbo.Garantia
        ADD nMarca INT NOT NULL
            CONSTRAINT DF_Garantia_nMarca DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nAnio') IS NULL
BEGIN
    ALTER TABLE dbo.Garantia
        ADD nAnio INT NOT NULL
            CONSTRAINT DF_Garantia_nAnio DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nValor') IS NULL
BEGIN
    ALTER TABLE dbo.Garantia
        ADD nValor MONEY NOT NULL
            CONSTRAINT DF_Garantia_nValor DEFAULT (0);
END;
GO

/*
 Valores temporales para que el API nuevo pueda convivir con las columnas
 antiguas durante el despliegue expand/contract. El script de normalización
 elimina estas columnas y sus defaults posteriormente.
*/
IF COL_LENGTH('dbo.GarantiaFoto', 'IdArticuloGarantia') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.default_constraints AS dc
       INNER JOIN sys.columns AS c ON c.default_object_id = dc.object_id
       WHERE dc.parent_object_id = OBJECT_ID('dbo.GarantiaFoto')
         AND c.name = 'IdArticuloGarantia'
   )
BEGIN
    ALTER TABLE dbo.GarantiaFoto
        ADD CONSTRAINT DF_GarantiaFoto_IdArticuloGarantia_Transicion
        DEFAULT (0) FOR IdArticuloGarantia;
END;
GO

IF COL_LENGTH('dbo.GarantiaFoto', 'nValor') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.default_constraints AS dc
       INNER JOIN sys.columns AS c ON c.default_object_id = dc.object_id
       WHERE dc.parent_object_id = OBJECT_ID('dbo.GarantiaFoto')
         AND c.name = 'nValor'
   )
BEGIN
    ALTER TABLE dbo.GarantiaFoto
        ADD CONSTRAINT DF_GarantiaFoto_nValor_Transicion
        DEFAULT (0) FOR nValor;
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nIdArticuloGarantia') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.default_constraints AS dc
       INNER JOIN sys.columns AS c ON c.default_object_id = dc.object_id
       WHERE dc.parent_object_id = OBJECT_ID('dbo.Garantia')
         AND c.name = 'nIdArticuloGarantia'
   )
BEGIN
    ALTER TABLE dbo.Garantia
        ADD CONSTRAINT DF_Garantia_nIdArticuloGarantia_Transicion
        DEFAULT (0) FOR nIdArticuloGarantia;
END;
GO

IF COL_LENGTH('dbo.Garantia', 'nIdFotoGarantia') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.default_constraints AS dc
       INNER JOIN sys.columns AS c ON c.default_object_id = dc.object_id
       WHERE dc.parent_object_id = OBJECT_ID('dbo.Garantia')
         AND c.name = 'nIdFotoGarantia'
   )
BEGIN
    ALTER TABLE dbo.Garantia
        ADD CONSTRAINT DF_Garantia_nIdFotoGarantia_Transicion
        DEFAULT (0) FOR nIdFotoGarantia;
END;
GO
