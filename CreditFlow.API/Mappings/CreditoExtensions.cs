using System;
using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Application.Requests;

namespace CreditFlow.API.Mappings
{
    public static class CreditoExtensions
    {
        public static CreditoRequest ToCreditoRequest(this Credito credito)
        {
            ArgumentNullException.ThrowIfNull(credito);

            return new CreditoRequest
            {
                nPrestamo = credito.NPrestamo,
                nProd = credito.NProd,
                nSubProd = credito.NSubProd,
                nPeriodo = credito.NPeriodo,
                nCobroEnAgencia = credito.NCobroEnAgencia ?? 0,
                nCodCred = credito.NCodCred,
                nCodAge = credito.NCodAge,
                fechaDesembolso = credito.DFecVig
            };
        }
    }
}
