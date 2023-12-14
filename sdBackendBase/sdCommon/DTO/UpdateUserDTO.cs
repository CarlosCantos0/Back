using System;
using System.Collections.Generic;
using System.Text;

/*
    DTO con los datos a actualizar de un usuario

*/
namespace sdCommon.DTO
{
    /*
        Clase base DTO para manejar la estructura de los datos actualizables de un Usuario gestionado por Identity (AppUser)  
    */  
    public class UpdateUserDTO
    {
        public System.Guid Id { get; set; }
        public string Alias { get; set; }

        public string Email { get; set; }

        public string[] Roles { get; set; }

        public string DefaultPage { get; set; }
    }
}
