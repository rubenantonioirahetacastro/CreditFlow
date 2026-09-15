using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Errors;
using CreditFlow.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Features.Credit.Web.UpdateCreditDecision;

public sealed class UpdateCreditDecisionHandler(DbNegocioContext context)
    : IUpdateCreditDecisionHandler
{
    public async Task ExecuteAsync(
        UpdateCreditDecisionRequest request,
        CancellationToken cancellationToken = default)
    {
        UpdateCreditDecisionValidator.Validate(request);

        var credit = await context.Creditos.FirstOrDefaultAsync(
            item =>
                item.NCodAge == request.NCodAge &&
                item.NCodCred == request.NCodCred,
            cancellationToken)
            ?? throw new ResourceNotFoundException(CreditErrors.NotFound);

        if (request.NEstado == UpdateCreditDecisionValidator.StatusApproved)
        {
            var isVerified = await context.VerificacionCreditos
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        item.NCodAge == request.NCodAge &&
                        item.NCodCred == request.NCodCred,
                    cancellationToken);

            if (!isVerified)
                throw new ResourceConflictException(CreditErrors.VerificationRequired);
        }

        credit.NEstado = request.NEstado;
        await context.SaveChangesAsync(cancellationToken);
    }
}
