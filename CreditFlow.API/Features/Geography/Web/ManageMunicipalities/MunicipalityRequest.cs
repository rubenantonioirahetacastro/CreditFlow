namespace CreditFlow.API.Features.Geography.Web.ManageMunicipalities;

public sealed class MunicipalityRequest
{
    public int IdMunicipio { get; set; }
    public int IdDepartamento { get; set; }
    public string CNombre { get; set; } = null!;
}
