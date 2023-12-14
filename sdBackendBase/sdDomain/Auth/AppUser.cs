using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modelos.Auth
{
    /*
        Clase que mapea los datos de usaurio que van a ser utilizados por Identity UserManager<AppUser>
    
        Partimos de IdentityUser<Guid> y ampliamos estas propiedades

        Importante que esta clase sea la que se le pasa en el startup y el Datacontext<AppUser, AppRole> en todos los proyectos 
        para poder tener centralizada el login y la gestión de usuarios

        Creo que faltaría añadir el nombre y apellidos y sería necesario consensuar qué otra información habría que extender de forma
        generalida para ser utilizada en cualquier proyecto

    */ 
    public class AppUser : IdentityUser<Guid>
    {
        [StringLength(50)]
        public string Alias { get; set; }
        //public string Correo { get; set; }

        //public string Contraseña { get; set; }


        public DateTime? LastAccess { get; set; }

        public bool Active { get; set; }

        public DateTime CreationDate { get; set; }
        
        public string? DefaultPage { get; set; }

        public AppUser()
        {
            CreationDate = DateTime.Now;
            Active = true;
        }
    }
}
