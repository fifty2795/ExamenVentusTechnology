using System;
using System.Collections.Generic;

namespace TaskManagement.API.Data.Models;

public partial class Tarea
{
    public int IdTarea { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int IdPrioridad { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFinalizacion { get; set; }

    public DateOnly FechaLimite { get; set; }

    public int IdEstatus { get; set; }

    public int IdUsuarioResponsable { get; set; }

    public virtual ICollection<AuditoriaEstatusTarea> AuditoriaEstatusTareas { get; set; } = new List<AuditoriaEstatusTarea>();

    public virtual CatEstatus IdEstatusNavigation { get; set; } = null!;

    public virtual CatPrioridade IdPrioridadNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioResponsableNavigation { get; set; } = null!;
}
