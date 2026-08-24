using System;
using System.Collections.Generic;

namespace CreditFlow.API.Features.SolicitudCredito.Domain.Model;

public partial class GarantiaFoto
{
    public int IdFoto { get; set; }

    public string VFoto { get; set; } = null!;

    public decimal NValor { get; set; }

    public int IdArticuloGarantia { get; set; }

    public int IdGarantia { get; set; }
}
