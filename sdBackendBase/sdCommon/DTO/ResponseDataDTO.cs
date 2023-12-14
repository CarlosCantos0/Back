using System;
using System.Collections.Generic;
using System.Text;


/*
    Clase para centralizar las respuestas desde un controlador

    IsSucces:           Indica si la respuesta ha sido exitosa
    Result:             Objeto con los datos de la respuesta
    DisplayMessage:     Mensaje informativo de la acción
    ErrorMessages:      Lista de errores detectados en la operación solicitada

*/
namespace sdCommon.DTO
{
    public class ResponseDataDTO
    {
        public bool IsSucces { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMessage { set; get; }
        public List<string> ErrorMessages { get; set; }


    }
}
