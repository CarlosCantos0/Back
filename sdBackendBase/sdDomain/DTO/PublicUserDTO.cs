using Modelos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdDomain.DTO
{
    /*
        Clase para mapear en un DTO de respuesta los datos públicos de un usuario (a partir de la definición AppUser de Identity)
    */
    public class PublicUserDTO
    {
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }
        
        public string NormalizedEmail { get; set; }

        public string NormalizedUserName { get; set; }

        public string UserName { get; set; }

        public Guid Id { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        // datos Extensión AppUser
        public string Alias { get; set; }

        public DateTime? LastAccess { get; set; }

        public bool Active { get; set; }

        public DateTime CreationDate { get; set; }

        public string? DefaultPage { get; set; }

        public IList<string> Roles { get; set; }

        public PublicUserDTO() : base() { }
        public PublicUserDTO(AppUser appUser)
        {
            LockoutEnd = appUser.LockoutEnd;
            TwoFactorEnabled = appUser.TwoFactorEnabled;
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed;
            PhoneNumber = appUser.PhoneNumber;
            EmailConfirmed = appUser.EmailConfirmed;
            NormalizedEmail = appUser.NormalizedEmail;
            NormalizedUserName = appUser.NormalizedUserName;
            UserName = appUser.UserName;
            Id = appUser.Id;
            LockoutEnabled = appUser.LockoutEnabled;
            AccessFailedCount = appUser.AccessFailedCount;
            Alias = appUser.Alias;
            LastAccess = appUser.LastAccess;
            Active = appUser.Active;
            CreationDate = appUser.CreationDate;
            DefaultPage = appUser.DefaultPage;
            Roles = new List<string>();
        }
    }
}
