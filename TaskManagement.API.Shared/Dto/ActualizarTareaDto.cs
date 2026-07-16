using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Dto
{
    public class ActualizarTareaDto : CrearTareaDto
    {
        public int IdTarea { get; set; }
    }
}
