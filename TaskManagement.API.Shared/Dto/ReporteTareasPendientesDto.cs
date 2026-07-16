using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Dto
{
    public class ReporteTareasPendientesDto
    {
        public int IdUsuario { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public int TotalPendientes { get; set; }

        public int TotalVencidas { get; set; }
    }
}
