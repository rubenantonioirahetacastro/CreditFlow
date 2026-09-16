using System.Text.Json.Serialization;

namespace CreditFlow.Web.Features.CreditEvaluation.Models;

public class EvaluacionCreditoDetalleDto
{
    [JsonPropertyName("cliente")] public ClienteEvaluacionDto Cliente { get; set; } = new();
    [JsonPropertyName("credito")] public CreditoEvaluacionDto Credito { get; set; } = new();
    [JsonPropertyName("negocio")] public NegocioEvaluacionDto? Negocio { get; set; }
    [JsonPropertyName("capacidadPago")] public CapacidadPagoEvaluacionDto? CapacidadPago { get; set; }
    [JsonPropertyName("documentacion")] public DocumentacionEvaluacionDto Documentacion { get; set; } = new();
    [JsonPropertyName("fiador")] public FiadorEvaluacionDto? Fiador { get; set; }
    [JsonPropertyName("garantia")] public GarantiaEvaluacionDto Garantia { get; set; } = new();
    [JsonPropertyName("verificaciones")] public List<VerificacionEvaluacionDto> Verificaciones { get; set; } = new();
}

public class ClienteEvaluacionDto
{
    [JsonPropertyName("idPersona")] public int IdPersona { get; set; }
    [JsonPropertyName("nombreCompleto")] public string NombreCompleto { get; set; } = string.Empty;
    [JsonPropertyName("cDocumento")] public string CDocumento { get; set; } = string.Empty;
    [JsonPropertyName("dFechaVencimientoDocumento")] public DateOnly DFechaVencimientoDocumento { get; set; }
    [JsonPropertyName("edad")] public int? Edad { get; set; }
    [JsonPropertyName("nEstadoCivil")] public int NEstadoCivil { get; set; }
    [JsonPropertyName("nProfesion")] public int NProfesion { get; set; }
    [JsonPropertyName("cCorreo")] public string? CCorreo { get; set; }
    [JsonPropertyName("cCelular")] public string CCelular { get; set; } = string.Empty;
    [JsonPropertyName("fotoUrl")] public string? FotoUrl { get; set; }
}

public class CreditoEvaluacionDto
{
    [JsonPropertyName("nCodAge")] public int NCodAge { get; set; }
    [JsonPropertyName("nCodCred")] public int NCodCred { get; set; }
    [JsonPropertyName("nCodLinea")] public int? NCodLinea { get; set; }
    [JsonPropertyName("agencia")] public string? Agencia { get; set; }
    [JsonPropertyName("nProd")] public int NProd { get; set; }
    [JsonPropertyName("nSubProd")] public int NSubProd { get; set; }
    [JsonPropertyName("subProducto")] public string? SubProducto { get; set; }
    [JsonPropertyName("nPrestamo")] public decimal NPrestamo { get; set; }
    [JsonPropertyName("nPeriodo")] public int NPeriodo { get; set; }
    [JsonPropertyName("plazoMeses")] public int PlazoMeses { get; set; }
    [JsonPropertyName("nNroCuotas")] public int NNroCuotas { get; set; }
    [JsonPropertyName("nTasaComp")] public decimal? NTasaComp { get; set; }
    [JsonPropertyName("nEstado")] public int NEstado { get; set; }
    [JsonPropertyName("estadoNombre")] public string? EstadoNombre { get; set; }
    [JsonPropertyName("dFecVig")] public DateTime DFecVig { get; set; }
    [JsonPropertyName("usuarioGestion")] public string? UsuarioGestion { get; set; }
}

public class NegocioEvaluacionDto
{
    [JsonPropertyName("cNombre")] public string CNombre { get; set; } = string.Empty;
    [JsonPropertyName("cDireccion")] public string CDireccion { get; set; } = string.Empty;
    [JsonPropertyName("cTelefono")] public string CTelefono { get; set; } = string.Empty;
    [JsonPropertyName("cSector")] public int CSector { get; set; }
    [JsonPropertyName("tipoNegocio")] public string? TipoNegocio { get; set; }
    [JsonPropertyName("tHoraInicio")] public string? THoraInicio { get; set; }
    [JsonPropertyName("tHoraCierre")] public string? THoraCierre { get; set; }
    [JsonPropertyName("cGeolocalizacion")] public string? CGeolocalizacion { get; set; }
    [JsonPropertyName("fotos")] public List<FotoEvaluacionDto> Fotos { get; set; } = new();
}

public class CapacidadPagoEvaluacionDto
{
    [JsonPropertyName("dGastosEducacion")] public decimal DGastosEducacion { get; set; }
    [JsonPropertyName("dGastosAlimentacion")] public decimal DGastosAlimentacion { get; set; }
    [JsonPropertyName("dGastosSalud")] public decimal DGastosSalud { get; set; }
    [JsonPropertyName("dOtrosGastos")] public decimal DOtrosGastos { get; set; }
    [JsonPropertyName("dOtrosIngresos")] public decimal DOtrosIngresos { get; set; }
}

public class DocumentacionEvaluacionDto
{
    [JsonPropertyName("fotos")] public List<FotoEvaluacionDto> Fotos { get; set; } = new();
}

public class FiadorEvaluacionDto
{
    [JsonPropertyName("nombreCompleto")] public string NombreCompleto { get; set; } = string.Empty;
    [JsonPropertyName("cDocumento")] public string CDocumento { get; set; } = string.Empty;
    [JsonPropertyName("cDireccion")] public string CDireccion { get; set; } = string.Empty;
    [JsonPropertyName("cCelular")] public string CCelular { get; set; } = string.Empty;
}

public class GarantiaEvaluacionDto
{
    [JsonPropertyName("nTipoGarantia")] public int NTipoGarantia { get; set; }
    [JsonPropertyName("tipoGarantia")] public string? TipoGarantia { get; set; }
    [JsonPropertyName("nMarca")] public int NMarca { get; set; }
    [JsonPropertyName("marca")] public string? Marca { get; set; }
    [JsonPropertyName("nAnio")] public int NAnio { get; set; }
    [JsonPropertyName("anio")] public string? Anio { get; set; }
    [JsonPropertyName("fotos")] public List<FotoGarantiaEvaluacionDto> Fotos { get; set; } = new();
    [JsonPropertyName("valorTotal")] public decimal ValorTotal { get; set; }
    [JsonPropertyName("coberturaPorcentaje")] public decimal? CoberturaPorcentaje { get; set; }
}

public class VerificacionEvaluacionDto
{
    [JsonPropertyName("cNombre")] public string CNombre { get; set; } = string.Empty;
    [JsonPropertyName("dFecha")] public DateTime DFecha { get; set; }
    [JsonPropertyName("nLatitud")] public decimal NLatitud { get; set; }
    [JsonPropertyName("nLongitud")] public decimal NLongitud { get; set; }
}

public class FotoEvaluacionDto
{
    [JsonPropertyName("idFoto")] public int IdFoto { get; set; }
    [JsonPropertyName("tipoFoto")] public int TipoFoto { get; set; }
    [JsonPropertyName("tipoFotoNombre")] public string? TipoFotoNombre { get; set; }
}

public class FotoGarantiaEvaluacionDto
{
    [JsonPropertyName("idFoto")] public int IdFoto { get; set; }
}
