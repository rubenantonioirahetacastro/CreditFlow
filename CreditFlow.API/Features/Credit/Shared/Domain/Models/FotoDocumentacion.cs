using System;
using System.Collections.Generic;

namespace CreditFlow.API.Features.Credit.Shared.Domain.Models;

public partial class FotoDocumentacion
{
    public int IdFoto { get; set; }

    public string? VFoto { get; set; }

    public int IdTipoDocumentacion { get; set; }

    public int IdDocumentacion { get; set; }
}
