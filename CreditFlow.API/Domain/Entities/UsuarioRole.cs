using System;
using System.Collections.Generic;

namespace CreditFlow.API.Domain.Entities;

public partial class UsuarioRole
{
    public int IdUsuarioRol { get; set; }

    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;

    public virtual UsuarioLogin IdUsuarioNavigation { get; set; } = null!;
}
