using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class CredGastoCuotaCambio
{
    public int Id { get; set; }

    public int NCodAge { get; set; }

    public int NCodCred { get; set; }

    public decimal NMontoNuevo { get; set; }
}
