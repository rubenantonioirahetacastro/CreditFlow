using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class CredLineaCredito
{
    public int NCodLinea { get; set; }

    public string CDescripcion { get; set; } = null!;

    public decimal NTasaCom { get; set; }

    public int NProd { get; set; }

    public int NSubProd { get; set; }

    public int NPlazoMin { get; set; }

    public int NPlazoMax { get; set; }

    public decimal NMontoMin { get; set; }

    public decimal NMontoMax { get; set; }

    public int? NNumPresMin { get; set; }

    public int? NNumPresMax { get; set; }

    public bool? BRefinan { get; set; }

    public string? CUser { get; set; }

    public bool BEstado { get; set; }
}
