﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class AuthorizationCodeViewModel
    {
        [Required]
        [DisplayName("Authorization Code")]
        public string AuthorizationCode { get; set; }
    }
}
