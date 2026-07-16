using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.API.Shared.Interfaces
{
    public interface ILogService
    {
        /// <summary>
        /// Metodo para escribir un mensaje de Error 
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        /// <param name="ex">Excepcion del error</param>
        void LogError(string message, Exception? ex = null);

        /// <summary>
        /// Metodo para escribir un mensaje de Informacion 
        /// </summary>
        /// <param name="message">Mensaje Informativo</param>
        void LogInfo(string message);

        /// <summary>
        /// Metodo para escribir un mensaje de Alerta
        /// </summary>
        /// <param name="message">Mensaje de Alerta</param>
        void LogWarning(string message);
    }
}
