using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Shared.Dto
{
    public class CrearTareaDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue)]
        public int IdPrioridad { get; set; }

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFinalizacion { get; set; }

        [Required]
        public DateOnly FechaLimite { get; set; }

        [Range(1, int.MaxValue)]
        public int IdEstatus { get; set; }

        [Range(1, int.MaxValue)]
        public int IdUsuarioResponsable { get; set; }
    }
}
