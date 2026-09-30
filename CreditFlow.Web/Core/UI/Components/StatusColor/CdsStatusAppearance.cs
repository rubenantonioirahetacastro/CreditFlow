namespace CreditFlow.Web.Core.UI.Components.StatusColor;

public sealed record CdsStatusAppearance(
    string ContentColor,
    string ContainerColor,
    string? DotColor = null,
    string? BorderColor = null)
{
    public string EffectiveDotColor => DotColor ?? ContentColor;

    public string EffectiveBorderColor => BorderColor ?? ContentColor;

    /// <summary>Resultado correcto de una operación (cronograma cuadrado, datos al día).</summary>
    public static CdsStatusAppearance Success { get; } = new(
        "var(--cds-success-text)", "var(--cds-success-surface)", "var(--cds-success-bright)", "var(--cds-success-border)");

    /// <summary>Resultado con error o inconsistencia (descuadre).</summary>
    public static CdsStatusAppearance Danger { get; } = new(
        "var(--cds-danger-text)", "var(--cds-danger-surface)", "var(--cds-danger)", "var(--cds-danger-border)");
}
