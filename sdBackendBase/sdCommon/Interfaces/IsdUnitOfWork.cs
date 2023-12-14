using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sdCommon.Interfaces
{
    /*
        Interfaz a implementar en una unidad de trabajo (UnitOfWork)
    */
    public interface IsdUnitOfWork
    {
        void BeginTransaction();

        void RollbackTransaction();

        void CommitTransaction();

        void SaveChanges();


        //***** Implementación de los metodos asincronos  *****//

        Task SaveChangesAsync();
        Task BeginTransactionAsync();
    }
}
