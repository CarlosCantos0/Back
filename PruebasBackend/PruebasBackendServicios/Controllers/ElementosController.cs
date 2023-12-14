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
using PruebasBackendModelo.Modelo;

namespace PruebasBackendServicios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ElementosController : sdController.Base.sdController
    {
        IElementosUnitOfWork _unitOfWork;
        public ElementosController(IElementosUnitOfWork unitOfWork) : base()
        {           
            _unitOfWork = unitOfWork;
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetElementos()
        {
            try
            {
                var data = await _unitOfWork.ElementosRepository.AllAsync();
                return Ok(data, "Lista de Elementos");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar obtener la lista de elementos", ex, true);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SetElementos([FromBody] List<Elemento> nuevosElementos)
        {
            try
            {
                // Elimina todos los elementos existentes asociados al esquema
                var idEsquema = nuevosElementos.FirstOrDefault()?.IdEsquema; // Tomamos el IdEsquema del primer elemento de la lista (si hay elementos)
                if (idEsquema.HasValue)
                {
                    var existingElements = await _unitOfWork.ElementosRepository.FilterAsync(e => e.IdEsquema == idEsquema.Value);
                    foreach (var existingElement in existingElements)
                    {
                        _unitOfWork.ElementosRepository.Delete(existingElement);
                    }
                }

                // Guarda los nuevos elementos
                foreach (var nuevoElemento in nuevosElementos)
                {
                    _unitOfWork.ElementosRepository.Add(nuevoElemento);
                }

                await _unitOfWork.SaveChangesAsync();  // Guarda los cambios en la base de datos

                return Ok("Elementos actualizados correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar actualizar los elementos", ex, true);
            }
        }

    }
}
