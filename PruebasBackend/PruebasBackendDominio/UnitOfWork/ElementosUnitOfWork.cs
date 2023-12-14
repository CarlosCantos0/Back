using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PruebasBackendDomain.Interfaces.UnitOfWork;
using PruebasBackendDomain.Context;
using PruebasBackendDominio.Repositories;
using PruebasBackendModelo.Modelo;
using PruebasBackendDominio.Interfaces.Repositories;
using System.Threading.Tasks;

namespace PruebasBackendDominio.UnitOfWork
{
    public class ElementosUnitOfWork : sdDomain.Base.sdUnitOfWork, IElementosUnitOfWork
    {
        IElementosRepositorio _ElementosRepository;

        public ElementosUnitOfWork(PruebasBackendContext context) : base(context)  {
            _ElementosRepository = new ElementosRepositorio(context);
        }
        public IElementosRepositorio ElementosRepository => _ElementosRepository;

        async Task<int> IElementosUnitOfWork.SaveChangesAsync()
        {
            return await _ElementosRepository.SaveChangesAsync();
        }
    }
}
