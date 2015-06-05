using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SRP_DAL
{
    interface IMinigame
    {
        int MGID { get;set;}
        bool EnableMediumDifficulty { get; set; }
        bool EnableHardDifficulty { get; set; }
        //DataSet FetchWithParent(int MGID);
    }
}
