namespace CreditFlow.API.Features.Credit.Shared.Contracts;
public class CrearSolicitudCreditoRequest
{
    public required PersonaRequest Persona { get; set; }
    public ConyugeRequest? Conyuge { get; set; }
    public FiadorRequest? Fiador { get; set; }
    public NegocioRequest? Negocio { get; set; }
    public CapacidadPagoRequest? CapacidadPago { get; set; }
    public required CreditoRequest Credito { get; set; }
    public GarantiaRequest? Garantia { get; set; }
    public List<CompraRequest>? Compra { get; set; }
    public List<VentaRequest>? Venta { get; set; }
    public List<FotoIdRequest>? FotoIds { get; set; }
    public List<FotoDocumentacionRequest>? FotoDocumentacions { get; set; }
    public List<FotoNegocioRequest>? FotoNegocios { get; set; }
    public List<GarantiaFotoRequest>? GarantiaFotos { get; set; }
}
