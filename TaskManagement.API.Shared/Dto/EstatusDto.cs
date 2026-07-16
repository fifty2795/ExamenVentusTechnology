using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Dto
{
    public class EstatusDto
    {
        public int IdEstatus { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
