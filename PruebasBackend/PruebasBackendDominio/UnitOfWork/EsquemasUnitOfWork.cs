using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PruebasBackendDomain.Interfaces.UnitOfWork;
using PruebasBackendDomain.Context;
using PruebasBackendDominio.Repositories;
using PruebasBackendModelo.Modelo;
using PruebasBackendDominio.Interfaces.Repositories;

namespace PruebasBackendDominio.UnitOfWork
{
    public class EsquemasUnitOfWork: sdDomain.Base.sdUnitOfWork, IEsquemasUnitOfWork
    {
        IEsquemasRepositorio _EsquemasRepository;

        public EsquemasUnitOfWork(PruebasBackendContext context) : base(context)  {            
            _EsquemasRepository = new EsquemasRepositorio(context);
        }
        public IEsquemasRepositorio EsquemasRepository => _EsquemasRepository;

    }
}
