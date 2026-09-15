using Microsoft.AspNetCore.Mvc;

namespace CreditFlow.API.Features.Payment.Mobile.RegisterPayment;

[Route("api/Pago")]
[ApiController]
public class RegisterPaymentController(IRegisterPaymentHandler handler) : ControllerBase
{
    [HttpPost("registrar")]
    public async Task<IActionResult> RegisterPayment(
        [FromBody] RegisterPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var updatedInstallments = await handler.ExecuteAsync(request, cancellationToken);
        return Ok(new
        {
            Mensaje = "Pago registrado exitosamente.",
            CuotasActualizadas = updatedInstallments
        });
    }
}
