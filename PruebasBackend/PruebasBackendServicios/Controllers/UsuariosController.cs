using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Modelos.Auth;
using PruebasBackendDomain.Interfaces.UnitOfWork;
using PruebasBackendDominio.Interfaces.Controllers;
using PruebasBackendDominio.Modelo.DTO;
using sdCommon.DTO;
using sdCommon.Enums;
using sdCommon.Interfaces;
using sdDomain.DTO;
using System;
using System.Threading.Tasks;

namespace PruebasBackendServicios.Controllers
{
    public class UsuariosController : sdController.Base.sdUsersController, IUsuariosController
    {
        // Unidad de trabajo para guardar los datos extendidos del usuario
        IUsuariosUnitOfWork _usuariosUnitOfWork;

        public UsuariosController(IUsuariosUnitOfWork usuariosUnitOfWork, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, 
                IConfiguration configuration, Func<TypeEmailServiceEnum, IEmailSender> serviceResolver) : 
            base(userManager, roleManager, configuration, serviceResolver)
        {
            _usuariosUnitOfWork = usuariosUnitOfWork;
        }

        // Anular el método GetUsuarios de la clase base porque no contempla los datos extendidos
        // Es más práctico usar el repositorio de usuarios y devolver todos los datos extendidos en una misma consulta
        // Aademás el método UserManager.Users.ToList<> no es asíncrono
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var data = await _usuariosUnitOfWork.UsuariosRepositorio.GetUsuarios();
                return Ok(data, "Lista de Usuarios");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar obtener la lista de usuarios", ex);
            }
        }

        // Edición de la estructura extendida de un usuario
        [HttpPut("[action]")]
        public async Task<IActionResult> EditarUsuario(UpdateUserExtDTO model)
        {
            // Necesitamos publicar el método pasándole el modelo 'UsuarioGrabarDTO' que contiene los datos extendidos del usuario (hereda de 'UsuarioGrabar') 
            // Y llamar al método base para que se guarde la parte que maneja Identity (AppUser)
            return await base.EditarUsuarioIdentity(model);
        }

        // Especialización para Editar los datos extendidos del Usuario
        protected override bool EditarUsuarioExt(UpdateUserDTO model, out string errorMessage)
        {
            errorMessage = null;
            bool result = false;
            try
            {
                // Comprobamos que realmente model tiene la estructura de 'UpdateUserDTO'
                UpdateUserExtDTO usuario = model as UpdateUserExtDTO;
                if (usuario == null)
                    errorMessage = "Los datos del modelo no se corresponden con la estructura esperada";
                else
                {
                    // Grabar datos específicos del usuario en el proyecto de pruebas
                    _usuariosUnitOfWork.UsuariosRepositorio.ActualizarUsuario(usuario.Id, usuario.Name, usuario.Surname); 
                    result = true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return result;
        }
        
        // Crear un usuario con la estructura extendida
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> CrearUsuario(NewUserExtDTO model)
        {
            // Necesitamos publicar el método pasándole el modelo 'NewUserExtDTO' que contiene los datos extendidos del usuario (hereda de 'NewUserDTO') 
            // Y llamar al método base para que se guarde la parte que maneja Identity (AppUser)
            return await base.CrearUsuarioIdentity(model);
        }

        // Especialización de la creación de un usuario extendido
        protected override bool CrearUsuarioExt(Guid Id, NewUserDTO model, out string errorMessage)
        {
            errorMessage = null;
            bool result = false;
            try
            {
                // Comprobamos que realmente model tiene la estructura de 'UsuarioGrabarDTO'
                NewUserExtDTO usuario = model as NewUserExtDTO;
                if (usuario == null)
                    errorMessage = "Los datos del modelo no se corresponden con la estructura esperada";
                else
                {
                    // Grabar datos específicos del usuario en el proyecto de pruebas
                    _usuariosUnitOfWork.UsuariosRepositorio.ActualizarUsuario(Id, usuario.Name, usuario.Surname);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return result;
        }

        // Devolver los datos extendidos de un usuario
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUsuario(Guid Id)
        {
            try
            {
                // Obtener datos de Identity (utilizano el método de la base)
                PublicUserDTO userDTO = await base.GetUsuarioIdentity(Id);
                PublicUserExtDTO userExtDTO = new PublicUserExtDTO();
                sdCommon.Clases.Utils.AssignProperties<PublicUserDTO>(userDTO, userExtDTO);

                // Obtener datos extendidos del usuario
                UserExtDTO user = _usuariosUnitOfWork.UsuariosRepositorio.GetUsuario(Id);
                userExtDTO.Name = user.Name;
                userExtDTO.Surname = user.Surname;

                // Devolver en una misma clase (PublicUserExtDTO) todos los datos extendidos
                return Ok(userExtDTO, "Datos del usuario");
            }
            catch (Exception ex)
            {
                return BadRequest("No se han podido obtener los datos públicos del usuario", ex);
            }
        }
    }
}
