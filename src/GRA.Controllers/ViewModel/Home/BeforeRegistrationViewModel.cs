using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Home
{
    public class BeforeRegistrationViewModel
    {
        public Site Site { get; set; }
        public string RegistrationOpens { get; set; }
        public string Email { get; set; }
        public string SignUpSource { get; set; }
    }
}
