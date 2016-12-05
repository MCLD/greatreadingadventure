using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public class ActivityResult
    {
        public int PointsEarned { get; set; }
        public User User { get; set; }
    }
}
