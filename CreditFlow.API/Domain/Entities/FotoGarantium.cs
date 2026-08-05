using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class FotoGarantium
{
    public int IdFoto { get; set; }

    public string? VFoto { get; set; }

    public int? IdGarantia { get; set; }
}
