
using PruebasBackendDominio.Interfaces.Repositories;
using sdCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackendDomain.Interfaces.UnitOfWork
{
    public interface IElementosUnitOfWork: IsdUnitOfWork
    {        
        public IElementosRepositorio ElementosRepository { get; }

        new Task<int> SaveChangesAsync();
    }
}
