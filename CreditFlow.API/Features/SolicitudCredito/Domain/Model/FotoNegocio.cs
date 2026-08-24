using System;
using System.Collections.Generic;

namespace CreditFlow.API.Features.SolicitudCredito.Domain.Model;

public partial class FotoNegocio
{
    public int IdFoto { get; set; }

    public string? VFoto { get; set; }

    public int NTipoFoto { get; set; }

    public int IdNegocio { get; set; }
}
