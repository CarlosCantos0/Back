using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PruebasBackendModelo.Modelo
{
    public partial class VEsquema
    {
        [Key]
        public int Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string Descripcion { get; set; }
    }
}
