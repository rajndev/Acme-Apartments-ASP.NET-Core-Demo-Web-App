﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Peach_Grove_Apartments_Demo_Project.Models
{
    public class AptUser : IdentityUser
    { 
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateRegistered { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zipcode { get; set; }
        public string SSN { get; set; }
        public string AptNumber { get; set; }
        public string AptPrice { get; set; }
        public virtual ICollection<Application> Applications { get; set; }
    }
}
