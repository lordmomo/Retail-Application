﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Models
{
    public class PersonDto
    {
        public string? Id { get; set; }
        public string? CustomerName { get; set; }

        public string? Address { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public double Balance { get; set; }

        public string? FilePath { get; set; }


    }
}
