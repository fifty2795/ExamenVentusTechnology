using System;
using System.Collections.Generic;

namespace TaskManagement.API.Data.Models;

public partial class CatPrioridade
{
    public int IdPrioridad { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
