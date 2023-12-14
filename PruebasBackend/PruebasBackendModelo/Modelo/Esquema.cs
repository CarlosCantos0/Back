using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PruebasBackendModelo.Modelo
{
    public partial class Esquema
    {
        public Esquema()
        {
            Elemento = new HashSet<Elemento>();
        }

        public int Id { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime FechaAlta { get; set; }

        public string Descripcion { get; set; }

        //public virtual AspNetUsers IdUsuarioNavigation { get; set; }
        public virtual ICollection<Elemento> Elemento { get; set; }
    }
}
