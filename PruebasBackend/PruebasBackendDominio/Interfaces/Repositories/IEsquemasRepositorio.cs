using sdCommon.Interfaces;
using PruebasBackendModelo.Modelo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebasBackendDominio.Interfaces.Repositories
{
    public interface IEsquemasRepositorio: IsdRepository<Esquema>
    {
        Task<IEnumerable<dynamic>> GetElementosEsquema(int idEsquema);
    }
}
