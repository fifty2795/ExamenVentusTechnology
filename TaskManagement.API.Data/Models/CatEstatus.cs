using System;
using System.Collections.Generic;

namespace TaskManagement.API.Data.Models;

public partial class CatEstatus
{
    public int IdEstatus { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<AuditoriaEstatusTarea> AuditoriaEstatusTareaIdEstatusAnteriorNavigations { get; set; } = new List<AuditoriaEstatusTarea>();

    public virtual ICollection<AuditoriaEstatusTarea> AuditoriaEstatusTareaIdEstatusNuevoNavigations { get; set; } = new List<AuditoriaEstatusTarea>();

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
