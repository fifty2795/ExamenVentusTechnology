using System;
using System.Collections.Generic;

namespace TaskManagement.API.Data.Models;

public partial class AuditoriaEstatusTarea
{
    public long IdAuditoria { get; set; }

    public int IdTarea { get; set; }

    public int IdEstatusAnterior { get; set; }

    public int IdEstatusNuevo { get; set; }

    public DateTime FechaCambio { get; set; }

    public string UsuarioBaseDatos { get; set; } = null!;

    public virtual CatEstatus IdEstatusAnteriorNavigation { get; set; } = null!;

    public virtual CatEstatus IdEstatusNuevoNavigation { get; set; } = null!;

    public virtual Tarea IdTareaNavigation { get; set; } = null!;
}
