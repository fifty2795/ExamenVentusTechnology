using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Dto
{
    public class FiltroTareaDto
    {
        public int? IdPrioridad { get; set; }

        public int? IdEstatus { get; set; }

        public int? IdUsuarioResponsable { get; set; }

        public DateTime? FechaInicial { get; set; }

        public DateTime? FechaFinal { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
