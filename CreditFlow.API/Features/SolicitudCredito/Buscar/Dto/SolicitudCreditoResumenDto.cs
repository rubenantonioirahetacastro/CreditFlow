namespace CreditFlow.API.Features.SolicitudCredito.Buscar.Dto;

public class SolicitudCreditoResumenDto
{
    public int IdPersona { get; set; }

    public int NCodAge { get; set; }

    public int NCodCred { get; set; }

    public string CDocumento { get; set; } = string.Empty;

    public string CNombres { get; set; } = string.Empty;

    public string CPrimerApellido { get; set; } = string.Empty;

    public string? CSegundoApellido { get; set; }

    public string? FotoUrl { get; set; }

    public string? CUsuarioGestion { get; set; }

    public int NProd { get; set; }

    public int NSubProd { get; set; }

    public decimal NPrestamo { get; set; }

    public int NEstado { get; set; }

    public DateTime DFecVig { get; set; }
}
