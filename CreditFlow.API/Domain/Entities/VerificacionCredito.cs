using System;

namespace CreditFlow.API.Domain.Entities;

public partial class VerificacionCredito
{
    public int IdVerificacion { get; set; }

    public int NCodCred { get; set; }

    public int NCodAge { get; set; }

    public int? IdEmpleado { get; set; }

    public string CNombre { get; set; } = null!;

    public DateTime DFecha { get; set; }

    public decimal NLatitud { get; set; }

    public decimal NLongitud { get; set; }
}
