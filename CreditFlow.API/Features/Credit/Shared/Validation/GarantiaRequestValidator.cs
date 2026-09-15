using CreditFlow.API.Features.Credit.Shared.Contracts;
using CreditFlow.API.Core.Errors;
using CreditFlow.API.Features.Credit.Shared.Errors;

namespace CreditFlow.API.Features.Credit.Shared.Validation;

public static class GarantiaRequestValidator
{
    public static void ValidateCustomerData(GarantiaRequest? guarantee)
    {
        if (guarantee == null)
            throw new RequestValidationException(CreditErrors.GuaranteeRequired);

        if (guarantee.NTipoGarantia <= 0)
            throw new RequestValidationException(CreditErrors.GuaranteeTypeRequired);

        if (guarantee.NMarca <= 0)
            throw new RequestValidationException(CreditErrors.GuaranteeBrandRequired);

        if (guarantee.NAnio <= 0)
            throw new RequestValidationException(CreditErrors.GuaranteeYearRequired);
    }
}
