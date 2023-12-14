using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PruebasBackendDomain.Context;
using PruebasBackendDominio.Interfaces.Repositories;
using PruebasBackendModelo.Modelo;
using sdDomain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackendDominio.Repositories
{
    public class EsquemasRepositorio: sdRepository<Esquema>, IEsquemasRepositorio { 
        public EsquemasRepositorio(PruebasBackendContext context) : base(context) {
        }

        public async Task<IEnumerable<dynamic>> GetElementosEsquema(int idEsquema)
        {
            try
            {
                string spName = "[dbo].getElementosPorEsquema";

                var p1 = new SqlParameter("@idEsquema", idEsquema);

                var data = await ExecuteProcedureAsync(spName, new[] { p1 });
                return data; 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
