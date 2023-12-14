using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modelos.Auth;
using sdCommon.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

/*
    Controlador que centraliza el login y la gestión del token de autenticación 
    Se puede usar en cualquier backend referenciando al proyecto 'sdController'
*/

namespace sdController.Base
{
    public class sdAuthController : sdController
    {
        private readonly UserManager<AppUser> _UserManager;
        private IConfiguration _Configuration;

        public sdAuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _UserManager = userManager;
            _Configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        //public virtual async Task<IActionResult> Login([FromBody] LoginModel login)
        public virtual async Task<IActionResult> Login(LoginDTO login)
        {
            IActionResult response = Unauthorized();

            try
            {
                var appUser = await _UserManager.FindByNameAsync(login.UserName);

                if (appUser == null)
                    return BadRequest("El usuario no existe");

                if (!await _UserManager.IsEmailConfirmedAsync(appUser))
                {
                    var reenvioUrl = Url.RouteUrl("DefaultApi", new { controller = "values", id = "123" });
                    string mensaje = "<p>Se debe confirmar el correo antes de ingresar</p>" +
                        "<p>Si desea volver a recibir el correo de confirmación pulse <a href=\"" + reenvioUrl + "\">aquí</a></p>";
                    return BadRequest("Email no confirmado", mensaje);
                }

                if (!appUser.Active)
                {
                    return BadRequest("El usuario está bloqueado");
                }

                if (!await _UserManager.CheckPasswordAsync(appUser, login.Password))
                    return BadRequest("Contrasena incorrecta");


                string tokenString = "";
                try
                {
                    tokenString = await BuildTokenAsync(appUser);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message, ex);
                }

                if (String.IsNullOrEmpty(tokenString))
                    return BadRequest("Error al generar el token de autenticación");

                // Actualizo la fecha del último acceso
                appUser.LastAccess = DateTime.Now;
                await _UserManager.UpdateAsync(appUser);

                appUser.Id = Guid.Empty;
                appUser.PasswordHash = "";
                appUser.ConcurrencyStamp = "";
                return Ok(tokenString, "¡Sesión iniciada con éxito, " + appUser.Alias + "!");
            }
            catch (Exception ex)
            {
                return BadRequest("Se ha producido un error al intentar iniciar sesión: " + ex.Message, ex, true);
            }
        }

        protected virtual async Task<string> BuildTokenAsync(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Auth:Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var roles = await _UserManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Actor, user.UserName),
                new Claim("roles", String.Join(',', roles)),
                new Claim("alias", String.IsNullOrEmpty(user.Alias) ? user.UserName : user.Alias),
                //new Claim("modulos", String.Join(',', modulos)),
            };

            JwtSecurityToken token = null;

            token = new JwtSecurityToken(
                issuer: _Configuration["Auth:Jwt:Issuer"],
                audience: _Configuration["Auth:Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_Configuration["Auth:Jwt:TokenExpirationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
    
}
