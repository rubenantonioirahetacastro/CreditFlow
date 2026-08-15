using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class LineaCatalogoAuxiliar
{
    public int Id { get; set; }

    public string? CDescripcion { get; set; }

    public int? NCatalogoCodigo { get; set; }

    public int? NProd { get; set; }

    public int? NSubProd { get; set; }

    public int? NPeriodicidad { get; set; }
}
