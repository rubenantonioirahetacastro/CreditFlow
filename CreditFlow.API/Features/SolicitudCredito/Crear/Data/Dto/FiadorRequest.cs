namespace CreditFlow.API.Features.SolicitudCredito.Data.Dto;

public class FiadorRequest
{
    public string CNombres { get; set; } = null!;

    public string CPrimerApellido { get; set; } = null!;

    public string CSegundoApellido { get; set; } = null!;

    public int NTipoDocumento { get; set; }

    public string CDocumento { get; set; } = null!;

    public string CDireccion { get; set; } = null!;

    public string? CTelefono { get; set; }

    public string CCelular { get; set; } = null!;
}
