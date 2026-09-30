using System.Globalization;
using System.Text;
using CreditFlow.Web.Shared.Catalog.Models;

namespace CreditFlow.Web.Features.Maintenance.Catalog.Models;

/// <summary>
/// Convenciones de la tabla CatalogoCodigo que la pantalla interpreta.
/// PENDIENTE DE CONFIRMAR con backend: la API guarda nEstados y nTipoCodigo como números sin significado definido.
/// Se asume nEstados 1 o vacío = Activo (cualquier otro valor = Inactivo; al desactivar se guarda 0)
/// y nTipoCodigo 1 = General, 2 = Sistema. Si cambian, se ajustan solo aquí.
/// </summary>
public static class CatalogoConvenciones
{
    public const int EstadoActivo = 1;
    public const int EstadoInactivo = 0;

    public const int TipoGeneral = 1;
    public const int TipoSistema = 2;

    /// <summary>La fila con nValor == nCodigo es el encabezado del catálogo (su clave técnica).</summary>
    public static bool EsEncabezado(CatalogoCodigoDto fila) => fila.NValor == fila.NCodigo;

    /// <summary>Activo si nEstados es 1 o está vacío (filas antiguas sin estado cargado).</summary>
    public static bool EsActivo(CatalogoCodigoDto fila) => fila.NEstados is null or EstadoActivo;

    public static bool EsSistema(CatalogoCodigoDto fila) => fila.NTipoCodigo == TipoSistema;

    /// <summary>«TIPO_DOCUMENTO» → «Tipo documento». La API solo guarda la clave, no un nombre visible.</summary>
    public static string NombreLegible(string clave)
    {
        var texto = clave.Replace('_', ' ').Trim().ToLower(CultureInfo.GetCultureInfo("es-SV"));
        return texto.Length == 0 ? clave : char.ToUpper(texto[0], CultureInfo.GetCultureInfo("es-SV")) + texto[1..];
    }

    /// <summary>Clave técnica: MAYÚSCULAS, sin tildes y con «_» en lugar de espacios o símbolos. «Tipo de garantía» → «TIPO_DE_GARANTIA».</summary>
    public static string ClaveTecnica(string nombre)
    {
        var sinTildes = new StringBuilder();
        foreach (var c in nombre.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sinTildes.Append(c);
        }

        var clave = new StringBuilder();
        foreach (var c in sinTildes.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant())
        {
            if (char.IsAsciiLetterOrDigit(c))
                clave.Append(c);
            else if (clave.Length > 0 && clave[^1] != '_')
                clave.Append('_');
        }

        return clave.ToString().Trim('_');
    }
}
