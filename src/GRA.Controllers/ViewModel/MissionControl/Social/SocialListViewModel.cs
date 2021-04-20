using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GRA.Controllers.ViewModel.MissionControl.Social
{
    public class ActiveSocials
    {
        public int LanguageId { get; set; }
        public int HeaderId { get; set; }
        public string Link { get; set; }
    }

    public class SocialListViewModel : PaginateViewModel
    {
        public ICollection<ActiveSocials> ActiveSocials { get; set; }
        public ICollection<Domain.Model.Language> Languages { get; set; }
        public ICollection<Domain.Model.SocialHeader> SocialHeaders { get; set; }

        public static object RowStatus(ITempDataDictionary tempData, int id)
        {
            return tempData?.ContainsKey($"RowStatus{id}") == true
                ? tempData[$"RowStatus{id}"]
                : null;
        }

        public object Status(int headerId)
        {
            var actives = ActiveSocials.Where(_ => _.HeaderId == headerId);
            if (actives.Any())
            {
                var sb = new StringBuilder();
                foreach(var active in actives)
                {
                    sb.Append(active.Link);
                }
                return sb.ToString();
            }
            return null;
        }
    }
}