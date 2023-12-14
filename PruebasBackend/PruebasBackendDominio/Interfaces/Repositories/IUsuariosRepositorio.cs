using PruebasBackendDominio.Modelo.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackendDominio.Interfaces.Repositories
{
    public interface IUsuariosRepositorio
    {
        bool ActualizarUsuario(Guid Id, string name, string surname);

        UserExtDTO GetUsuario(Guid Id);
        Task<IEnumerable<dynamic>> GetUsuarios();

    }
}
