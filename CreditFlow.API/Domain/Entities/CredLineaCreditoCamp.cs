using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class CredLineaCreditoCamp
{
    public int Id { get; set; }

    public int NCodLinea { get; set; }

    public int NCodCamp { get; set; }
}
