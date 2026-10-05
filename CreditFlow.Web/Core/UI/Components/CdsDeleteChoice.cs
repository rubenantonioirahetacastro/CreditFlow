namespace CreditFlow.Web.Core.UI.Components;

/// <summary>Respuesta de <see cref="CdsDeleteDialog"/> (nula si se cancela).</summary>
public enum CdsDeleteChoice
{
    /// <summary>Alternativa segura: el registro queda inactivo y se puede reactivar.</summary>
    Desactivar,

    /// <summary>Borra el registro definitivamente.</summary>
    Eliminar
}
