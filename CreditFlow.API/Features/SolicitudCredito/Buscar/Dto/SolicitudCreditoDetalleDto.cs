namespace CreditFlow.API.Features.SolicitudCredito.Buscar.Dto;

public class SolicitudCreditoDetalleDto
{
    public required PersonaDetalleDto Persona { get; set; }
    public ConyugeDetalleDto? Conyuge { get; set; }
    public NegocioDetalleDto? Negocio { get; set; }
    public CapacidadPagoDetalleDto? CapacidadPago { get; set; }
    public DocumentacionDetalleDto Documentacion { get; set; } = new();
    public FiadorDetalleDto? Fiador { get; set; }
    public GarantiaDetalleDto Garantia { get; set; } = new();
    public required CreditoDetalleDto Credito { get; set; }
}
