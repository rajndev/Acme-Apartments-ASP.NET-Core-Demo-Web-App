﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcmeApartments.Web.ViewModels
{
    public class NewMaintenanceRequestViewModel
    {
        [Required]
        [MaxLength(10000)]
        [Display(Name = "Problem Description")]
        public string ProblemDescription { get; set; }

        [DisplayName("Permitted to enter the apartment?")]
        public bool isAllowedToEnter { get; set; }
    }
}