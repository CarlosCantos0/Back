using PruebasBackendDominio.Interfaces.Repositories;
using sdCommon.Interfaces;

namespace PruebasBackendDomain.Interfaces.UnitOfWork
{
    public interface IUsuariosUnitOfWork : IsdUnitOfWork
    {
        IUsuariosRepositorio UsuariosRepositorio { get; }


    }
}
