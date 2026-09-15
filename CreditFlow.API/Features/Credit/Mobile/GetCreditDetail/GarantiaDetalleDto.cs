namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class GarantiaDetalleDto
{
    public int IdGarantia { get; set; }
    public int NTipoGarantia { get; set; }
    public int NMarca { get; set; }
    public int NAnio { get; set; }
    public decimal NValor { get; set; }
    public List<FotoGarantiaDto> Fotos { get; set; } = new();
}
