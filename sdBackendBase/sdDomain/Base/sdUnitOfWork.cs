using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sdCommon.Interfaces;


/*
 
    Clase base desde la que heredar para manejar las unidades de trabajo (UnitOfWorks)

    Gestiona y centraliza las operaciones básicas de actualización a nivel de unidad de trabajo (agrupación de repositorios)

*/

namespace sdDomain.Base
{
    public abstract class sdUnitOfWork : IsdUnitOfWork, IDisposable
    {
        private DbContext _context;

        public sdUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public virtual void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public virtual void CommitTransaction()
        {
            _context.Database.CommitTransaction();
        }

        public virtual void RollbackTransaction()
        {
            _context.Database.RollbackTransaction();
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();            
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
