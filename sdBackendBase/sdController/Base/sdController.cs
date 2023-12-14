using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using sdCommon.DTO;

/*
    Controlador base que encapsula las respuestas OK y BadRequest a través de la clase ResponseData, 
    permitiendo devolver los datos de forma estructurada 

*/

namespace sdController.Base
{
    [Route("api/[controller]")]
    [EnableCors("EnableCORS")]
    [ApiController]
    [Authorize]
    public abstract class sdController : ControllerBase
    {                    
        protected OkObjectResult Ok(object result, string displayMessage = null)
        {
            ResponseDataDTO responseData = new ResponseDataDTO
            {
                IsSucces = true,
                DisplayMessage = displayMessage,
                Result = result
            };
            return base.Ok(responseData);
        }

        protected BadRequestObjectResult BadRequest(object error, string displayMessage = null)
        {
            ResponseDataDTO responseData = new ResponseDataDTO
            {
                IsSucces = false,
                Result = error,
                DisplayMessage = displayMessage,
            };
            return base.BadRequest(responseData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        protected BadRequestObjectResult BadRequest(string displayMessage = null, Exception exception = null, bool showExceptionDetails = false)
        {
            ResponseDataDTO responseData = new ResponseDataDTO
            {                
                IsSucces = false,
                Result = displayMessage,
            };

            if (exception == null)
                responseData.DisplayMessage = displayMessage;
            else
            {
                if (!string.IsNullOrEmpty(displayMessage))
                    responseData.DisplayMessage = displayMessage + ". " + exception.Message;
                else
                    responseData.DisplayMessage = exception.Message;

                if (showExceptionDetails)
                {
                    responseData.ErrorMessages = new List<string> { exception.Message };
                    if (exception.InnerException != null)
                        responseData.ErrorMessages.Add(exception.InnerException.Message);
                }
            }
            return base.BadRequest(responseData);
        }
    }
}
