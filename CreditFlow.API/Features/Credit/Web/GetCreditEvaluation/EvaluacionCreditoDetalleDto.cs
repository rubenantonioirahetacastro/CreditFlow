namespace CreditFlow.API.Features.Credit.Web.GetCreditEvaluation;

public class EvaluacionCreditoDetalleDto
{
    public required ClienteEvaluacionDto Cliente { get; set; }
    public required CreditoEvaluacionDto Credito { get; set; }
    public NegocioEvaluacionDto? Negocio { get; set; }
    public CapacidadPagoEvaluacionDto? CapacidadPago { get; set; }
    public DocumentacionEvaluacionDto Documentacion { get; set; } = new();
    public FiadorEvaluacionDto? Fiador { get; set; }
    public GarantiaEvaluacionDto Garantia { get; set; } = new();
    public List<VerificacionEvaluacionDto> Verificaciones { get; set; } = new();
}

public class ClienteEvaluacionDto
{
    public int IdPersona { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CDocumento { get; set; } = string.Empty;
    public DateOnly DFechaVencimientoDocumento { get; set; }
    public int? Edad { get; set; }
    public int NEstadoCivil { get; set; }
    public int NProfesion { get; set; }
    public string? CCorreo { get; set; }
    public string CCelular { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }

    /// <summary>Fotos del documento de identidad (FotoIds). Se sirven con GET api/BuscarSolicitudCredito/foto/persona/{idFoto}.</summary>
    public List<FotoEvaluacionDto> FotosDocumento { get; set; } = new();
}

public class CreditoEvaluacionDto
{
    public int NCodAge { get; set; }
        public int NCodCred { get; set; }
        public int? NCodLinea { get; set; }
    public string? Agencia { get; set; }
    public int NProd { get; set; }
    public int NSubProd { get; set; }
    public string? SubProducto { get; set; }
    public decimal NPrestamo { get; set; }
    public int NPeriodo { get; set; }
    public int PlazoMeses { get; set; }
    public int NNroCuotas { get; set; }
    public decimal? NTasaComp { get; set; }
    public int NEstado { get; set; }
    public string? EstadoNombre { get; set; }
    public DateTime DFecVig { get; set; }
    public string? UsuarioGestion { get; set; }
}

public class NegocioEvaluacionDto
{
    public string CNombre { get; set; } = string.Empty;
    public string CDireccion { get; set; } = string.Empty;
    public string CTelefono { get; set; } = string.Empty;
    public int CSector { get; set; }
    public string? TipoNegocio { get; set; }
    public string? THoraInicio { get; set; }
    public string? THoraCierre { get; set; }
    public string? CGeolocalizacion { get; set; }
    public List<FotoEvaluacionDto> Fotos { get; set; } = new();
}

public class CapacidadPagoEvaluacionDto
{
    public decimal DGastosEducacion { get; set; }
    public decimal DGastosAlimentacion { get; set; }
    public decimal DGastosSalud { get; set; }
    public decimal DOtrosGastos { get; set; }
    public decimal DOtrosIngresos { get; set; }
}

public class DocumentacionEvaluacionDto
{
    public List<FotoEvaluacionDto> Fotos { get; set; } = new();
}

public class FiadorEvaluacionDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string CDocumento { get; set; } = string.Empty;
    public string CDireccion { get; set; } = string.Empty;
    public string CCelular { get; set; } = string.Empty;
}

public class GarantiaEvaluacionDto
{
    public int NTipoGarantia { get; set; }
    public string? TipoGarantia { get; set; }
    public int NMarca { get; set; }
    public string? Marca { get; set; }
    public int NAnio { get; set; }
    public string? Anio { get; set; }
    public List<FotoGarantiaEvaluacionDto> Fotos { get; set; } = new();
    public decimal ValorTotal { get; set; }
    public decimal? CoberturaPorcentaje { get; set; }
}

public class VerificacionEvaluacionDto
{
    public string CNombre { get; set; } = string.Empty;
    public DateTime DFecha { get; set; }
    public decimal NLatitud { get; set; }
    public decimal NLongitud { get; set; }
}

public class FotoEvaluacionDto
{
    public int IdFoto { get; set; }
    public int TipoFoto { get; set; }
    public string? TipoFotoNombre { get; set; }
}

public class FotoGarantiaEvaluacionDto
{
    public int IdFoto { get; set; }
}
