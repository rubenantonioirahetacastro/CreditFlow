namespace CreditFlow.API.Features.EvaluacionCredito.Dto;


public class DatosPersonaGestionResponse
{
    public int IdPersona { get; set; }

    public string CDocumento { get; set; } = string.Empty;

    public string CNombres { get; set; } = string.Empty;

    public string CPrimerApellido { get; set; } = string.Empty;

    public string? CSegundoApellido { get; set; }

    public string? CCorreo { get; set; }

    public string CTelefono { get; set; } = string.Empty;

    public string CCelular { get; set; } = string.Empty;

    public string? VFotoPerfil { get; set; }

    public string? CUsuarioGestion { get; set; }
}
