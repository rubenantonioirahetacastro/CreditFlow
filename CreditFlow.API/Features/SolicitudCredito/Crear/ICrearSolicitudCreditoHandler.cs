using CreditFlow.API.Domain.Entities;
using CreditFlow.API.Features.SolicitudCredito.Domain.Model;

namespace CreditFlow.API.Features.SolicitudCredito;

public interface ICrearSolicitudCreditoHandler
{
    Task<int> EjecutarAsync(
        List<FotoId>? fotoIds,
        List<FotoDocumentacion>? fotoDocumentacions,
        List<GarantiaFoto>? fotoGarantias,
        List<FotoNegocio>? fotoNegocios,
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
