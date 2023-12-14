using Modelos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdDomain.DTO
{
    /*
        Clase para mapear en un DTO de respuesta los datos públicos de un Rol (a partir de la definición AppRole de Identity)
    */
    public class PublicRoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public PublicRoleDTO() : base() { }

        public PublicRoleDTO(AppRole appRole)
        {
            Id = appRole.Id;
            Name = appRole.Name;
            NormalizedName = appRole.NormalizedName;
            Description = appRole.Description;
            CreationDate = appRole.CreationDate;
        }
    }
}
