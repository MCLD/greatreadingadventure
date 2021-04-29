﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class BundlesDetailViewModel
    {
        public AvatarBundle Bundle { get; set; }
        public string Action { get; set; }
        [Required(ErrorMessage = "Please select items for the bundle")]
        public string ItemsList { get; set; }
        public SelectList Layers { get; set; }
        public ICollection<Trigger> TriggersAwardingBundle { get; set; }
        public string Search { get; set; }
    }
}
