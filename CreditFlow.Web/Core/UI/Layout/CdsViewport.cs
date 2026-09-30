using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CreditFlow.Web.Core.UI.Layout;

public static class CdsViewport
{
    /// <summary>
    /// Ajusta la raíz de una página <c>cds-page--fill</c> al alto visible (wwwroot/js/simulador.js).
    /// Es un ajuste cosmético: si el script no está disponible (p. ej. caché del navegador) o el circuito se cierra,
    /// se ignora y la página sigue funcionando con el alto definido en CSS. Una excepción aquí terminaría el circuito
    /// de Blazor y dejaría la página sin datos.
    /// </summary>
    public static async Task FitAsync(this IJSRuntime js, ElementReference pageRoot)
    {
        try
        {
            await js.InvokeVoidAsync("cdsFitViewport", pageRoot);
        }
        catch (JSException)
        {
        }
        catch (JSDisconnectedException)
        {
        }
        catch (TaskCanceledException)
        {
        }
    }
}
