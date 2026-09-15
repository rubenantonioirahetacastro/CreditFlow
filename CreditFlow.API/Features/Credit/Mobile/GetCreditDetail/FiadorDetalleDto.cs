namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class FiadorDetalleDto
{
    public int IdFiador { get; set; }
    public string CNombres { get; set; } = string.Empty;
    public string CPrimerApellido { get; set; } = string.Empty;
    public string CSegundoApellido { get; set; } = string.Empty;
    public int NTipoDocumento { get; set; }
    public string CDocumento { get; set; } = string.Empty;
    public string CDireccion { get; set; } = string.Empty;
    public string? CTelefono { get; set; }
    public string CCelular { get; set; } = string.Empty;
}
