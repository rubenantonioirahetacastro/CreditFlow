namespace CreditFlow.API.Features.Credit.Mobile.GetCreditDetail;

public class PersonaDetalleDto
{
    public int IdPersona { get; set; }
    public int NTipoDocumento { get; set; }
    public string CDocumento { get; set; } = string.Empty;
    public DateOnly DFechaExpedicion { get; set; }
    public DateOnly DFechaVencimiento { get; set; }
    public int NDepartamentoDoc { get; set; }
    public int NMunicipioDoc { get; set; }
    public string CNombres { get; set; } = string.Empty;
    public string CPrimerApellido { get; set; } = string.Empty;
    public string? CSegundoApellido { get; set; }
    public int NSexo { get; set; }
    public int NNacionalidad { get; set; }
    public DateOnly DFechaNacimiento { get; set; }
    public int NDepartamentoNacimiento { get; set; }
    public int NMunicipioNacimiento { get; set; }
    public int NEstadoCivil { get; set; }
    public int NProfesion { get; set; }
    public int NEscolaridad { get; set; }
    public string? CCorreo { get; set; }
    public string CDireccion { get; set; } = string.Empty;
    public string CTelefono { get; set; } = string.Empty;
    public string CCelular { get; set; } = string.Empty;
    public string? VFotoPerfil { get; set; }
    public List<FotoDto> Fotos { get; set; } = new();
}
