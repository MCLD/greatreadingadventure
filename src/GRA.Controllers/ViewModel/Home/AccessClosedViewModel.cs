using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Home
{
    public class AccessClosedViewModel
    {
        public bool CollectEmail { get; set; }
        public string Email { get; set; }
        public string SignUpSource { get; set; }
    }
}
