namespace CreditFlow.API.Features.SolicitudCredito.Data.Dto;

public class GarantiaFotoRequest
{
    public int IdFoto { get; set; }
    public string? VFoto { get; set; }
    public decimal NValor { get; set; }
    public int IdArticuloGarantia { get; set; }
    public IFormFile? Archivo { get; set; }
}
