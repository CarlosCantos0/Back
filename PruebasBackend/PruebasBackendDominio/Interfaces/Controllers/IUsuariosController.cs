using Microsoft.AspNetCore.Mvc;
using PruebasBackendDominio.Modelo.DTO;
using sdCommon.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackendDominio.Interfaces.Controllers
{
    public interface IUsuariosController
    {
        Task<IActionResult> GetUsuarios();

        Task<IActionResult> EditarUsuario(UpdateUserExtDTO model);

        Task<IActionResult> CrearUsuario(NewUserExtDTO model);
        
        Task<IActionResult> GetUsuario(Guid Id);
        
        Task<IActionResult> EliminarUsuario(Guid Id);
        
        Task<IActionResult> ReenviarCorreoConfirmacionEmail(EmailDTO email);
        
        Task<ActionResult> RecuperarContrasenya(EmailDTO model);        
        
        Task<ActionResult> ResetPassword(ResetPasswordDTO model);
        
        Task<IActionResult> GetRol(Guid Id);
        
        // Este método se podría implementar de forma asíncrona y dejar de usar el base (RoleManager<AppRole>)
        IActionResult GetRoles();

        Task<IActionResult> EliminarRol(Guid Id);
        
        Task<IActionResult> CrearRol(NewRoleDTO newRole);
    }
}
