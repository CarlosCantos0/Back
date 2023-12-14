using Microsoft.Data.SqlClient;
using PruebasBackendDomain.Context;
using PruebasBackendDominio.Interfaces.Repositories;
using PruebasBackendDominio.Modelo.DTO;
using sdDomain.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackendDominio.Repositories
{
    public class UsuariosRepository: sdRepository<object>, IUsuariosRepositorio
    {
        public UsuariosRepository(PruebasBackendContext context): base(context)
        {
        }

        public bool ActualizarUsuario(Guid Id, string name, string surname)
        {
            try
            {
                string spName = "[users].[spUpdateExt]";

                var p1 = new SqlParameter("@Id", Id);
                var p2 = new SqlParameter("@Name", name);
                var p3 = new SqlParameter("@Surname", surname);

                ExecuteProcedureNonQuery(spName, new[] { p1, p2, p3 });
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public UserExtDTO GetUsuario(Guid Id)
        {
            try
            {
                string spName = "[users].[spGetUserExt]";
                var p1 = new SqlParameter("@Id", Id);
                List<UserExtDTO> lstUsuario = ExecuteProcedure<UserExtDTO>(spName, new[] { p1 }, null);
                return lstUsuario[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetUsuarios()
        {
            try
            {
                string spName = "[users].spGetUsuarios";
                var data = await ExecuteProcedureAsync(spName);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
