using sdCommon.DTO;

namespace PruebasBackendDominio.Modelo.DTO
{
    // Extensión DTO de la definición de los datos del usuario a grabar que no son gestionados por identity AppUser (nombre y apellidos)
    public class UpdateUserExtDTO : UpdateUserDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
