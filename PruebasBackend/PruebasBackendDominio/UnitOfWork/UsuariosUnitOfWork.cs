using PruebasBackendDomain.Context;
using PruebasBackendDomain.Interfaces.UnitOfWork;
using PruebasBackendDominio.Interfaces.Repositories;
using PruebasBackendDominio.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PruebasBackendDominio.UnitOfWork
{
    public class UsuariosUnitOfWork: sdDomain.Base.sdUnitOfWork, IUsuariosUnitOfWork
    {
        IUsuariosRepositorio _usuariosRepositorio;
        public UsuariosUnitOfWork(PruebasBackendContext context) : base(context)
        {
            _usuariosRepositorio = new UsuariosRepository(context);
        }

        public IUsuariosRepositorio UsuariosRepositorio => _usuariosRepositorio;
    }
}
