using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class CredLineaCreditoAge
{
    public int Id { get; set; }

    public int NCodLinea { get; set; }

    public int NCodAge { get; set; }
}
