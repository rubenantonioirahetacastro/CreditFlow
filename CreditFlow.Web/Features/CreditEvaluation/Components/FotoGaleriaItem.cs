namespace CreditFlow.Web.Features.CreditEvaluation.Components;

/// <summary>Foto a mostrar en <see cref="FotoGaleria"/> con su pie opcional.</summary>
public sealed record FotoGaleriaItem(int IdFoto, string? Pie = null);
