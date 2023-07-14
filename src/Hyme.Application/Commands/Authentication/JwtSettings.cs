﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Application.Commands.Authentication
{
    public class JwtSettings
    {

        public const string SectionName = "JwtSettings";
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpiryDays { get; set; }
    }
}
