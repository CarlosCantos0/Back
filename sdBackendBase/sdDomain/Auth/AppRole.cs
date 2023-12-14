using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modelos.Auth
{
    /*
        Clase báse para manejar un rol de Identity
    */ 
    public class AppRole : IdentityRole<Guid>
    {
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
    }
}
