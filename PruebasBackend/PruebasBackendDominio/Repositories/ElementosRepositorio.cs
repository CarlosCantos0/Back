using PruebasBackendDomain.Context;
using PruebasBackendDominio.Interfaces.Repositories;
using PruebasBackendModelo.Modelo;
using sdDomain.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackendDominio.Repositories
{
    public class ElementosRepositorio: sdRepository<Elemento>, IElementosRepositorio
    {
        public ElementosRepositorio(PruebasBackendContext context) : base(context) {
        }
    }
}
