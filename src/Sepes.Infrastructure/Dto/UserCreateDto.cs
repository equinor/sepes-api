﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto
{
    public class UserCreateDto
    {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }
    }
}
