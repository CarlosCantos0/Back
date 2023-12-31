﻿using System;
using System.Collections.Generic;
using System.Text;

namespace sdCommon.DTO
{
    public class NewUserDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Alias { get; set; }

        public string[] Roles { get; set; }

        public string DefaultPage { get; set; }

    }
}
