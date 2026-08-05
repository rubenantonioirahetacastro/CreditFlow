using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class CredFeriado
{
    public int NIdFeriado { get; set; }

    public DateTime DFecha { get; set; }

    public string CDescripcion { get; set; } = null!;

    public bool BEstado { get; set; }
}
