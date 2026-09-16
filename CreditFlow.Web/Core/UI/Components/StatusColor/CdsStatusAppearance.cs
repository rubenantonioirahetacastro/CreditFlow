namespace CreditFlow.Web.Core.UI.Components.StatusColor;

public sealed record CdsStatusAppearance(
    string ContentColor,
    string ContainerColor,
    string? DotColor = null,
    string? BorderColor = null)
{
    public string EffectiveDotColor => DotColor ?? ContentColor;

    public string EffectiveBorderColor => BorderColor ?? ContentColor;
}
