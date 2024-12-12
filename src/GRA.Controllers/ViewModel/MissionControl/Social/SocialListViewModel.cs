using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GRA.Controllers.ViewModel.MissionControl.Social
{
    public class ActiveSocials
    {
        public string Badge { get; set; }
        public int HeaderId { get; set; }
        public string Link { get; set; }
    }

    public class SocialListViewModel : PaginateViewModel
    {
        public SocialListViewModel()
        {
            ActiveSocials = new List<ActiveSocials>();
            Languages = new List<Domain.Model.Language>();
            SocialHeaders = new List<Domain.Model.SocialHeader>();
        }

        public ICollection<ActiveSocials> ActiveSocials { get; }
        public ICollection<Domain.Model.Language> Languages { get; }
        public ICollection<Domain.Model.SocialHeader> SocialHeaders { get; }

        public static object RowStatus(ITempDataDictionary tempData, int id)
        {
            return tempData?.ContainsKey($"RowStatus{id}") == true
                ? tempData[$"RowStatus{id}"]
                : null;
        }
    }
}
