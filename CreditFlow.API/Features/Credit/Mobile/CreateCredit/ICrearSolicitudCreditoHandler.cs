using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.Credit.Shared.Domain.Models;

namespace CreditFlow.API.Features.Credit.Mobile.CreateCredit;

public interface ICrearSolicitudCreditoHandler
{
    Task<int> EjecutarAsync(
        List<FotoId>? fotoIds,
        List<FotoDocumentacion>? fotoDocumentacions,
        List<GarantiaFoto>? fotoGarantias,
        List<FotoNegocio>? fotoNegocios,
        Garantium garantia,
        Persona persona,
        Conyuge? conyuge,
        Fiador? fiador,
        Negocio? negocio,
        CapacidadPago? capacidadPago,
        List<Compra>? compra,
        List<Venta>? venta,
        Credito credito
    );
}
