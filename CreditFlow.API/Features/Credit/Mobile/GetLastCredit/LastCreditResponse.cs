using System;
using System.Text.Json.Serialization;

namespace CreditFlow.API.Features.Credit.Mobile.GetLastCredit
{
    public class LastCreditResponse
    {
        [JsonPropertyName("ultimocred")]
        public LastCreditDetailResponse Ultimocred { get; set; } = new();

        [JsonPropertyName("resumenultcred")]
        public LastCreditSummaryResponse Resumenultcred { get; set; } = new();
    }

    public class LastCreditDetailResponse
    {
        [JsonPropertyName("prestamo")]
        public decimal Prestamo { get; set; }

        [JsonPropertyName("cuota")]
        public decimal NroCuota { get; set; }

        [JsonPropertyName("ncuota")]
        public int Ncuotas { get; set; }

        [JsonPropertyName("fechaVen")]
        public DateTime? FechaVencimiento { get; set; }

        [JsonPropertyName("ncodcred")]
        public int Ncodcred { get; set; }

        [JsonPropertyName("bReprestamo")]
        public bool BReprestamo { get; set; }

        [JsonPropertyName("bRefinanciamiento")]
        public bool BRefinanciamiento { get; set; }
    }

    public class LastCreditSummaryResponse
    {
        [JsonPropertyName("pagado")]
        public decimal Pagado { get; set; }

        [JsonPropertyName("pendiente")]
        public decimal Pendiente { get; set; }

        [JsonPropertyName("cuotasrestante")]
        public int CuotasRestantes { get; set; }

        [JsonPropertyName("cuotasltotales")]
        public int CuotaslTotales { get; set; }

        [JsonPropertyName("cuotaspagadas")]
        public int CuotasPagadas { get; set; }
    }
}
