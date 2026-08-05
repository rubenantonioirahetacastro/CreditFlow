namespace CreditFlow.API.Application.Requests
{
    public class PagoRequest
    {
        public int NCodAge { get; set; }
        public int NCodCred { get; set; }
        public decimal MontoAbonado { get; set; }
    }
}