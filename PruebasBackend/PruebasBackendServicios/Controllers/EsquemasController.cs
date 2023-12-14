using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sdCommon.Interfaces;
using PruebasBackendDominio.UnitOfWork;
using PruebasBackendDomain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using sdDomain.Base;
using PruebasBackendModelo.Modelo;

namespace PruebasBackendServicios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EsquemasController : sdController.Base.sdController
    {
        IEsquemasUnitOfWork _unitOfWork;
        public EsquemasController(IEsquemasUnitOfWork unitOfWork) : base()
        {           
            _unitOfWork = unitOfWork;
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetElementosEsquema(int idEsquema)
        {
            try
            {
                var data = await _unitOfWork.EsquemasRepository.GetElementosEsquema(idEsquema);
                return Ok(data, "Lista de Elementos del esquema " + idEsquema);
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar obtener la lista de Esquemas", ex, true);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetEsquemas()
        {
            // Prueba llamando a método de EE
            try
            {
                var data = await _unitOfWork.EsquemasRepository.AllAsync();
                return Ok(data, "Lista de Esquemas");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar obtener la lista de Esquemas", ex);
            }
        }

        [HttpDelete("[action]/{idEsquema}")]
        public async Task<IActionResult> DeleteEsquema(int idEsquema)
        {
            try
            {
                // Verifica si el esquema existe antes de intentar eliminarlo
                var esquema = await _unitOfWork.EsquemasRepository.GetByIdAsync(idEsquema);
                if (esquema == null)
                {
                    return NotFound($"Esquema con ID {idEsquema} no encontrado");
                }

                // Elimina el esquema y guarda los cambios
                _unitOfWork.EsquemasRepository.Delete(esquema);
                await _unitOfWork.SaveChangesAsync();

                return Ok($"Esquema con ID {idEsquema} eliminado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Se ha producido un error al intentar eliminar el esquema con ID {idEsquema}", ex, true);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AgregarEsquema([FromBody] VEsquema nuevoEsquema)
        {
            try
            {
                // Mapea los datos de la vista VEsquema a tu entidad Esquema
                var esquema = new Esquema
                {
                    IdUsuario = nuevoEsquema.IdUsuario,
                    Descripcion = nuevoEsquema.Descripcion,
                    FechaAlta = DateTime.Now // O establece la fecha de alta según tus requerimientos
                };

                // Añade el esquema a tu DbSet y guarda los cambios en la base de datos
                _unitOfWork.EsquemasRepository.Add(esquema);
                await _unitOfWork.SaveChangesAsync();

                return Ok("Esquema agregado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar el esquema: {ex.Message}");
            }
        }

    }
}
