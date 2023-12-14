using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Modelos.Auth;
using sdCommon.DTO;
using sdCommon.Enums;
using sdCommon.Interfaces;
using sdDomain.Clases;
using sdDomain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

/*
     Controlador base para centralizar las operaciones sobre el usuario gestionado por Identity UserManager<AppUser> y RolManager<AppRole>

    Se debe heredar para extender los métodos de actualización y demás caracterísiticas concretas que no estén contempladas en la estructura AppUser
*/


namespace sdController.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class sdUsersController : sdController
    {
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<AppRole> _RoleManager;
        private IConfiguration _Configuration;
        private sdEmailSender _EmailSender;

        public sdUsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration, 
            Func<TypeEmailServiceEnum, IEmailSender> serviceResolver): base()
        {
            _UserManager = userManager;
            _RoleManager = roleManager;
            _Configuration = configuration;
            _EmailSender = serviceResolver(TypeEmailServiceEnum.Naturgy) as sdEmailSender;            
        }
        
        // Modificar datos extendidos del usuario en clases heredadas
        protected abstract bool EditarUsuarioExt(UpdateUserDTO model, out string errorMessage);

        // Obtener datos del usuario (estructura AppUser de Identity). Llamar desde clases heredadas para rellenar datos extendidos
        protected async Task<PublicUserDTO> GetUsuarioIdentity(Guid Id)
        {
            var appUser = await _UserManager.FindByIdAsync(Id.ToString());
            if (appUser == null)
                throw new Exception ("El usuario especificado no existe");

            var publicUserDTO = new PublicUserDTO(appUser)
            {
                Roles = await _UserManager.GetRolesAsync(appUser)
            };
            return publicUserDTO;
        }


        // Eliminar un usuario (Como los datos extendidos del usuario se asume que están en la misma tabla users.AspNetUsers, no hay que hacer nada en subclases)
        [HttpDelete("[action]")]
        public virtual async Task<IActionResult> EliminarUsuario(Guid Id)
        {
            try
            {
                AppUser appUser = await _UserManager.FindByIdAsync(Id.ToString());
                if (appUser == null)
                    return BadRequest("El usuario especificado no existe");
                IdentityResult result = await _UserManager.DeleteAsync(appUser);
                if (result.Succeeded)
                    return Ok(result, "El usuario ha sido eliminado con éxito");
                else
                    return BadRequest(result, "No se ha podido eliminar el usuario");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar eliminar el usuario", ex);
            }
        }

        // Edición de los datos de un usuario       
        protected async Task<IActionResult> EditarUsuarioIdentity(UpdateUserDTO model)
        {
            // Uso de transacción para guardar la parte de usuarios definida en Identity (estructura AppUser), los roles asignado al usuario
            // y los datos de usuario extendidos si se implementa el método 'UpdateUserExt' en clases heredadas
            using (var transaction = new TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    AppUser user = await _UserManager.FindByIdAsync(model.Id.ToString());
                    if (user == null)
                    {
                        return BadRequest("No se ha encontrado el usuario para actualizar.");
                    }

                    user.Email = model.Email;
                    user.Alias = model.Alias;
                    user.DefaultPage = model.DefaultPage;

                    var result = await _UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var roles = await _UserManager.GetRolesAsync(user);
                        foreach (var rol in roles)
                        {
                            await _UserManager.RemoveFromRoleAsync(user, rol);
                        }
                        await _UserManager.AddToRolesAsync(user, model.Roles);

                        // grabar otros datos (La llamada que hacía Javi para mantener la tabla auxiliar)
                        //_servicio.UsuarioRepository.SetOtrosDatosUsuario(user.Id.ToString(), model.idCanal, model.idEmpresa);
                        if (EditarUsuarioExt(model, out string errorMessage))
                        {
                            transaction.Complete();
                            return Ok(true, "Los datos del usuario y los roles asignados han sido guardados con éxito.");
                        }
                        else
                            return BadRequest(errorMessage, "No se han podido guardar los datos extendidos del usuario. Se cancelará toda la operación de actualización.");
                    }
                    else
                    {
                        return BadRequest(result.Errors.ToList().ToString(), "No se han podido guardar los datos del usuario. Se cancelará la operación de actualización.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        // Insertar datos extendidos del usuario en clases heredadas
        protected abstract bool CrearUsuarioExt(Guid Id, NewUserDTO model, out string errorMessage);
        
        // Añadir un usuario
        protected async Task<IActionResult> CrearUsuarioIdentity(NewUserDTO model)
        {
            using (var transaction = new TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (await _UserManager.FindByNameAsync(model.UserName) != null)
                        return BadRequest("Ya existe un usuario con el nombre de usuario: " + model.UserName);

                    AppUser appUser = new AppUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Alias = model.Alias,
                        EmailConfirmed = true
                    };

                    var result = await _UserManager.CreateAsync(appUser, model.Password);
                    if (result.Succeeded)
                    {
                        transaction.Complete();
                        return Ok(true, "El usuario ha sido creado con éxito.");
                    }
                    else
                    {
                        return BadRequest(result.Errors.ToList().ToString(), "No se ha podido crear el usuario.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        /// Solicitud de código para reenviar el correo de confirmarción de correo
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ReenviarCorreoConfirmacionEmail(EmailDTO email)
        {
            if (String.IsNullOrEmpty(email.Email))
                return BadRequest("No se ha proporcionado el correo electrónico", "Email no confirmado");

            var user = await _UserManager.FindByEmailAsync(email.Email);
            if (user == null)
                return BadRequest("El correo electrónico especificado no existe", "Email no confirmado");
            
            if (await _UserManager.IsEmailConfirmedAsync(user))
                return BadRequest("El correo electrónico especificado ya fue confirmado en otra ocasión", "Email no conformado");

            var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);
            var codePass = await _UserManager.GeneratePasswordResetTokenAsync(user);
            string serverUrl = _Configuration.GetSection("Server").Value;
            string nombreWeb = _Configuration.GetSection("NombreWeb").Value;

            var confirmarEmailUrl = new Uri(serverUrl + "/Usuarios/ConfirmarEmail?userId=" + user.Id.ToString() + "&email=" + WebUtility.UrlEncode(user.Email) + "&code=" + WebUtility.UrlEncode(code) + "&codePass=" + WebUtility.UrlEncode(codePass));
            var reenvioConfirmarUrl = new Uri(serverUrl + "/Usuarios/RecuperarPassword");

            var loginUrl = new Uri(serverUrl + "/login");

            string correo =
                "<p>Reenvio de confirmación para el usuario <strong>" + user.UserName + "</strong> asociado a esta cuenta de correo para el acceso a la web de " + nombreWeb + ".</p>" +
                "<p>Por favor, confirme su cuenta y establezca su contraseña pulsando en el siguiente enlace <a href=\"" + confirmarEmailUrl + "\">Aquí</a></p>" +
                "<p>Por su seguridad, este enlace tiene una vigencia limitada, tras lo cual dejará de funcionar.</p>" +
                "<p>En caso de que se supere el plazo o haya un error en el correo, puede volver a generarse la confirmación introduciendo su correo <a href=\"" + reenvioConfirmarUrl + "\">Aquí</a>.</p>";

            await _EmailSender.SendEmailAsync(email.Email, "Portal " + nombreWeb + ": Confirmar cuenta de usuario", correo);

            return Ok("El email: " + email + "ha sido confirmado", "Email confirmado con éxito");
        }

        /// Solicitud para recuperar la contraseña y obtener el código
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> RecuperarContrasenya(EmailDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Modelo no valido", "No se ha podido restablecer la contraseña");

            var user = await _UserManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("El email no existe", "No se ha podido restablecer la contraseña");
            else if (!(await _UserManager.IsEmailConfirmedAsync(user)))
            {
                await this.ReenviarCorreoConfirmacionEmail(model);
                return Ok("Email Pendiente de confirmación", "Se ha enviado un email solicitando la confirmación del email");
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await _UserManager.GeneratePasswordResetTokenAsync(user);

            string serverUrl = _Configuration.GetSection("Server").Value;
            string nombreWeb = _Configuration.GetSection("NombreWeb").Value;

            var recuperarPasswordUrl = new Uri(serverUrl + "/Usuarios/ResetPassword?userId=" + user.Id.ToString() + "&code=" + WebUtility.UrlEncode(code));
            string mensaje = "Por favor, reestablezca la contraseña del usuario <strong>" + user.UserName + "</strong> pulsando <a href=\"" + recuperarPasswordUrl + "\">aquí</a>";

            await _EmailSender.SendEmailAsync(model.Email, "Portal " + nombreWeb + ": Recuperar contraseña", mensaje);
            return Ok("Se ha enviado un email a: " + model.Email + " para restablecer la contraseña", "Email de recuperación de contraseña enviado");
        }

        // Resetear contraseña de usuario
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Modelo no valido", "No se ha podido resetar la contraseña");

            var user = await _UserManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("El email no existe", "No se ha podido resetear la contraseña");

            var result = await _UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
                return Ok(result, "La contraseña se ha resetado con éxito");
            else
            {
                string errores = "";
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordTooShort")
                    {
                        errores = "La contraseña debe tener al menos 5 caracteres";
                        break;
                    }

                    else if (error.Code == "PasswordRequiresDigit")
                    {
                        errores = "La contraseña requiere del algún dígito (0-9)";
                        break;
                    }
                    else
                    {
                        errores = error.Description;
                    }

                }
                string errorMensaje = "No se pudo resetear la contraseña: " + errores;
                return BadRequest(errorMensaje, "No se ha podido resetear la contraseña");
            }
        }


        // Obtener datos del rol (estructura AppRol de Identity). Llamar desde clases heredadas
        [HttpGet("[action]")]
        public async Task<IActionResult> GetRol(Guid Id)
        {
            var appRole = await _RoleManager.FindByIdAsync(Id.ToString());
            if (appRole == null)
                return BadRequest("El rol especificado no existe");

            PublicRoleDTO publicRoleDTO = new PublicRoleDTO(appRole);
            return Ok(publicRoleDTO, "Datos del rol especificado");
        }

        // Como norma general los datos de AppRole no sería necesario extenderlos más allá de nuestra definición AppRole
        // Por ese motivo podemos usar RoleManager<AppRole> para obtener la lista completa de role
        // No he visto la menera de hacer que este método sea asíncrono ¿?
        [HttpGet("[action]")]
        public IActionResult GetRoles()
        {
            try
            {
                var data = _RoleManager.Roles.ToList().OrderBy(x => x.Name);
                return Ok(data, "Lista de Roles");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar obtener la lista de Roles", ex);
            }
        }


        // Eliminar un Rol 
        [HttpDelete("[action]")]
        public virtual async Task<IActionResult> EliminarRol(Guid Id)
        {
            try
            {
                AppRole appRole = await _RoleManager.FindByIdAsync(Id.ToString());
                if (appRole == null)
                    return BadRequest("El rol especificado no existe");
                IdentityResult result = await _RoleManager.DeleteAsync(appRole);
                if (result.Succeeded)
                    return Ok(result, "El rol ha sido eliminado con éxito");
                else
                    return BadRequest(result, "No se ha podido eliminar el rol");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar eliminar el rol", ex);
            }
        }

        // Crear un Rol 
        [HttpPost("[action]")]
        public virtual async Task<IActionResult> CrearRol(NewRoleDTO newRole)
        {
            try
            {
                if (await _RoleManager.FindByNameAsync(newRole.Name) != null)
                    return BadRequest("El rol especificado ya existe");
                
                AppRole appRole = new AppRole()
                {
                    Name = newRole.Name,
                    CreationDate = DateTime.Now,
                    Description = newRole.Description
                };
                await _RoleManager.CreateAsync(appRole);
                return Ok("El rol se ha creado con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar crear el rol", ex);
            }
        }
    }
}
