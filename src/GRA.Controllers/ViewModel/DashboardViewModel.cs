using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public int CurrentPointTotal { get; set; }
        public string AvatarPath { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
    }
}
