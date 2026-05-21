using YuvaTravel.Base.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace YuvaTravel.Data.Dtos.Profiles
{

    public class InformationDto
    {
        [Required]
        public Guid UserGuid { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "姓")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "名")]
        public string FirstName { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [MaxLength(2)]
        [Display(Name = "稱謂")]
        public string Salutation { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        public InformationDto()
        {
        }
    }
}
