
using Microsoft.Data.SqlClient;
using PruebasBackendDominio.Interfaces.Repositories;
using sdCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PruebasBackendDomain.Interfaces.UnitOfWork
{
    public interface IEsquemasUnitOfWork: IsdUnitOfWork
    {        
        public IEsquemasRepositorio EsquemasRepository { get; }

    }
}
