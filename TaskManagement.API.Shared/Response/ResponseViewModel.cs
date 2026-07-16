using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Response
{
    public class ResponseViewModel<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public int Code { get; set; }        
    }

    public class ResponseViewModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ErrorViewModel? Error { get; set; }
    }
}
