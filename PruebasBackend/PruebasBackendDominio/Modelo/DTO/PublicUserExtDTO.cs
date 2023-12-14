
using sdDomain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PruebasBackendDominio.Modelo.DTO
{
    public class PublicUserExtDTO: PublicUserDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
