using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdCommon.Interfaces;
using System.Data;
using System.Reflection;
using sdDomain.Extensiones;
using Microsoft.Data.SqlClient;
using System.Threading;

/*
    Clase base para gestionar un reposirio sobre una entidad genérica TEntity  
    Extensión de métodos para ejecutar procedimientos almacenados y la conversión de Datatables en JSON         
*/

namespace sdDomain.Base
{
    public class sdRepository<TEntity> : IsdRepository<TEntity> where TEntity : class
    {
        protected DbContext _db;

        public sdRepository(DbContext context)
        {
            _db = context; 
        }



        #region Implementacion de la interfaz

        public virtual void Add(TEntity entity)
        {
            var newEntry = _db.Set<TEntity>().Add(entity);
        }

        public virtual IQueryable<TEntity> All()
        {
            return _db.Set<TEntity>().AsNoTracking().AsQueryable<TEntity>();
        }

        public virtual IQueryable<TEntity> All(Expression<Func<TEntity, TEntity>> select)
        {
            return _db.Set<TEntity>().Select(select).AsNoTracking().AsQueryable();
        }

        public virtual async Task<IEnumerable<TEntity>> AllAsync()
        {
            return await _db.Set<TEntity>().AsNoTracking().ToListAsync<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> AllAsync(Expression<Func<TEntity, TEntity>> select)
        {
            return await _db.Set<TEntity>().Select(select).AsNoTracking().ToListAsync<TEntity>();
        }



        public virtual bool Contains(Expression<Func<TEntity, bool>> predicate)
        {
            return _db.Set<TEntity>().AsNoTracking().Count<TEntity>(predicate) > 0;
        }

        public virtual async Task<bool> ContainsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _db.Set<TEntity>().AsNoTracking().CountAsync(predicate) > 0;
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null)
                return;
            _db.Set<TEntity>().Remove(entity);
        }

        public virtual void Delete(int[] ids)
        {
            foreach (int id in ids)
            {
                var entity = GetById(id);
                if (entity == null)
                    return;
                _db.Set<TEntity>().Remove(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            _db.Set<TEntity>().Remove(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var objects = Filter(predicate);
            foreach (var obj in objects)
                _db.Set<TEntity>().Remove(obj);
        }

        public virtual IEnumerable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            return _db.Set<TEntity>().Where<TEntity>(predicate).AsNoTracking().AsQueryable<TEntity>();
        }

        public virtual IEnumerable<TEntity> Filter(Expression<Func<TEntity, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? _db.Set<TEntity>().Where<TEntity>(filter).AsNoTracking().AsQueryable() : _db.Set<TEntity>().AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public virtual IEnumerable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _db.Set<TEntity>().Where(predicate).AsNoTracking().ToListAsync<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> filter, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? _db.Set<TEntity>().Where<TEntity>(filter).AsNoTracking().AsQueryable() : _db.Set<TEntity>().AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);            
            return await _resetSet.ToListAsync();
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _db.Set<TEntity>().AsNoTracking().FirstOrDefault<TEntity>(predicate);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public virtual TEntity GetById(int id)
        {
            return _db.Set<TEntity>().Find(id);
        }


        public virtual TEntity GetById(int? id)
        {
            if (id == null)
                return null;
            else
                return _db.Set<TEntity>().Find(id);
        }

        public virtual TEntity GetById(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            DbSet<TEntity> dbSet = _db.Set<TEntity>();

            var item = dbSet.Find(id);

            return item;
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _db.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            DbSet<TEntity> dbSet = _db.Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                dbSet.Include(includeProperty);
            }

            return await dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(int? id)
        {
            if (id == null)
                return null;
            else
                return await _db.Set<TEntity>().FindAsync(id);
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> expression)
        {
            return All().AsNoTracking().FirstOrDefault(expression);
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var dbSet = _db.Set<TEntity>();
            foreach (var property in includeProperties)
                dbSet.Include(property);

            return dbSet.FirstOrDefault(expression);
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression)
        {
            return All().AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var dbSet = All();
            foreach (var property in includeProperties)
                dbSet.Include(property);

            var item = await dbSet.AsNoTracking().FirstOrDefaultAsync(expression);
            return item;
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                var entry = _db.Entry(entity);
                _db.Set<TEntity>().Attach(entity);
                entry.State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual int SaveChanges()
        {
            return _db.SaveChanges();
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return _db.SaveChanges(acceptAllChangesOnSuccess);
        }
        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }


        #endregion

        #region Trabajar con Datatables y procedimientos almacenados

        protected DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        protected DataTable ToDataTable<T>(IEnumerable<T> items, string field)
        {
            DataTable dataTable = new DataTable();
            //Get all the properties by using reflection   
            dataTable.Columns.Add(field, typeof(int));
            foreach (T item in items)
            {
                dataTable.Rows.Add(item);
            }

            return dataTable;
        }

        protected SqlParameter GetStructuredParameter<T>(IEnumerable<T> paramList, string paramName, string SQLType = "dbo.ListaID", string field = "ID")
        {
            var parameter = new SqlParameter(paramName, SqlDbType.Structured);
            parameter.TypeName = SQLType;
            // creo el datatable
            DataTable dt = null;
            if (SQLType == "dbo.ListaID")
                dt = ToDataTable<T>(paramList.ToList(), field);
            else
                dt = ToDataTable<T>(paramList.ToList());

            parameter.Value = dt;

            return parameter;
        }

        protected async Task<IEnumerable<dynamic>> ExecuteProcedureAsync(string storeProcedure, SqlParameter[] parameters, int? timeout = null)
        {
            return await _ExecuteProcedureAsync(storeProcedure, parameters, timeout);
        }

        protected async Task<IEnumerable<dynamic>> ExecuteProcedureAsync(string storeProcedure, int? timeout = null)
        {
            return await _ExecuteProcedureAsync(storeProcedure, null, timeout);
        }

        protected IEnumerable<dynamic> ExecuteProcedure(string storeProcedure, SqlParameter[] parameters, int? timeout = null)
        {
            return _ExecuteProcedure(storeProcedure, parameters, timeout);
        }

        protected IEnumerable<dynamic> ExecuteProcedure(string storeProcedure, int? timeout = null)
        {
            return _ExecuteProcedure(storeProcedure, null, timeout);
        }

        protected int ExecuteProcedureNonQuery(string storeProcedure, SqlParameter[] parameters, int? timeout = null)
        {
            return _ExecuteProcedureNonQuery(storeProcedure, parameters, timeout);
        }

        protected async Task<int> ExecuteProcedureNonQueryAsync(string storeProcedure, SqlParameter[] parameters, int? timeout = null)
        {
            return await _ExecuteProcedureNonQueryAsync(storeProcedure, parameters, timeout);
        }

        private int _ExecuteProcedureNonQuery(string storeProcedure, SqlParameter[] parameters, int? timeout)
        {
            using (var connection = new SqlConnection(_db.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(storeProcedure, connection))
                {
                    if (timeout != null)
                        command.CommandTimeout = timeout ?? 15;

                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        private Task<int> _ExecuteProcedureNonQueryAsync(string storeProcedure, SqlParameter[] parameters, int? timeout)
        {
            using (var connection = new SqlConnection(_db.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(storeProcedure, connection))
                {
                    if (timeout != null)
                        command.CommandTimeout = timeout ?? 15;

                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    return command.ExecuteNonQueryAsync();
                }
            }
        }

        public List<T> ExecuteProcedure<T>(string storeProcedure, SqlParameter[] parameters, int? timeout) where T : class
        {
            IEnumerable<dynamic> result = _ExecuteProcedure(storeProcedure, parameters, timeout);
            List<T> lst = sdCommon.Clases.Utils.ConvertToType<List<T>>(result);
            return lst;
        }


        private IEnumerable<dynamic> _ExecuteProcedure(string storeProcedure, SqlParameter[] parameters, int? timeout)
        {
            using (var connection = new SqlConnection(_db.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(storeProcedure, connection))
                {
                    if (timeout != null)
                        command.CommandTimeout = timeout ?? 15;

                    command.CommandType = CommandType.StoredProcedure;


                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }


                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);

                            if (ds.Tables.Count == 1)
                            {
                                return ds.Tables[0].ToExpandoObjectList();
                            }
                            else
                            {
                                var list = new List<dynamic>();
                                foreach (DataTable table in ds.Tables)
                                {
                                    list.Add(table.ToExpandoObjectList());
                                }
                                return list;
                            }
                        }
                    }
                }
            }
        }




        private async Task<IEnumerable<dynamic>> _ExecuteProcedureAsync(string storeProcedure, SqlParameter[] parameters, int? timeout)
        {
            using (var connection = new SqlConnection(_db.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(storeProcedure, connection))
                {
                    if (timeout != null)
                        command.CommandTimeout = timeout ?? 15;

                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);

                            if (ds.Tables.Count == 1)
                            {
                                return ds.Tables[0].ToExpandoObjectList();
                            }
                            else
                            {
                                var list = new List<dynamic>();
                                foreach (DataTable table in ds.Tables)
                                {
                                    list.Add(table.ToExpandoObjectList());
                                }
                                return (IEnumerable<dynamic>)Task.FromResult(list);
                            }
                        }
                    }
                }
            }
        }

        protected string ExecuteProcedureJson(string storeProcedure, SqlParameter[] parameters = null)
        {
            string resultado = "";
            using (var connection = new SqlConnection(_db.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(storeProcedure, connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    using (var dataReader = command.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                resultado += dataReader.GetString(0);
                            }
                        }

                    }
                }
            }
            return resultado;
        }




        #endregion


    }
}
