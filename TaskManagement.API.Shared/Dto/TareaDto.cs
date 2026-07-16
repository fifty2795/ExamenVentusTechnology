using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.API.Shared.Dto
{
    public class TareaDto
    {
        public int IdTarea { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public int IdPrioridad { get; set; }

        public string Prioridad { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFinalizacion { get; set; }

        public DateOnly FechaLimite { get; set; }

        public int IdEstatus { get; set; }

        public string Estatus { get; set; } = string.Empty;

        public int IdUsuarioResponsable { get; set; }

        public string UsuarioResponsable { get; set; } = string.Empty;
    }
}
