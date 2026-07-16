using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Response
{
    public class ResponseHelper
    {
        public static ResponseViewModel<T> CrearRespuestaExito<T>(T? data, string mensajeExito)
        {
            var response = new ResponseViewModel<T>
            {
                Success = true,
                Message = mensajeExito,
                Data = data,
                Code = 200
            };

            return response;
        }

        public static ResponseViewModel<T> CrearRespuestaError<T>(string mensajeError, int code)
        {
            return new ResponseViewModel<T>
            {
                Success = false,
                Message = mensajeError,
                Code = code                
            };
        }
    }
}
