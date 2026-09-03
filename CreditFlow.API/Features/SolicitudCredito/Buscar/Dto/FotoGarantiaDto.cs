namespace CreditFlow.API.Features.SolicitudCredito.Buscar.Dto;

public class FotoGarantiaDto
{
    public string Ruta { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int IdArticuloGarantia { get; set; }
}
