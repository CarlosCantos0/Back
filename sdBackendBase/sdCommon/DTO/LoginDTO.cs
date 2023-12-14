using System;
using System.Collections.Generic;
using System.Text;

namespace sdCommon.DTO
{
    /*
        Clase DTO para las peticiones de login
    */ 
    public class LoginDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
