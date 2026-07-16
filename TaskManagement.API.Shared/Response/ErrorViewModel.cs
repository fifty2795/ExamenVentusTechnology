using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Response
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        
        public string? StackTrace { get; set; }
        
        public string? Source { get; set; }

        public int? ErrorCode { get; set; }
        
        public ErrorViewModel(string message, int? errorCode = null)
        {
            Message = message;            
            ErrorCode = errorCode;
        }
    }
}
