﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class MailSendViewModel
    {
        [Range(1, Int32.MaxValue)]
        public int Id { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
