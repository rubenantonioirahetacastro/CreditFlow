namespace CreditFlow.Web.Core.Navigation;

/// <summary>
/// Nivel de la ruta de la barra superior («Inicio / Otorgamiento / … / Página»).
/// Con <see cref="Href"/> se muestra como enlace (p. ej. la bandeja desde la que se abrió un detalle);
/// sin él es un grupo del menú, solo texto.
/// </summary>
public sealed record PagePathItem(string Text, string? Href = null)
{
    public static implicit operator PagePathItem(string text) => new(text);
}
