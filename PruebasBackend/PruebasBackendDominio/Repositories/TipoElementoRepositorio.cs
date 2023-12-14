using PruebasBackendDomain.Context;
using PruebasBackendDominio.Interfaces.Repositories;
using PruebasBackendModelo.Modelo;
using sdDomain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PruebasBackendDominio.Repositories
{
    public class TipoElementoRepositorio: sdRepository<TipoElemento>, ITipoElementoRepositorio
    {
        public TipoElementoRepositorio(PruebasBackendContext context) : base(context) {
        }
    }
}
