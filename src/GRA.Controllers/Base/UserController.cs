using GRA.Controllers.Filter;
using GRA.Domain.Repository.Extensions;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(UserFilter), Order = 2)]
    [ServiceFilter(typeof(NotificationFilter))]
    public abstract class UserController : Controller
    {
        public UserController(ServiceFacade.Controller context) : base(context)
        {
        }

        protected async Task<DynamicAvatarDetails> GetDynamicAvatarDetailsAsync(string dynamicAvatar,
            DynamicAvatarService dynamicAvatarService)
        {
            bool problem = false;
            Dictionary<int, int> avatarLayerElement = null;
            if (!string.IsNullOrEmpty(dynamicAvatar))
            {
                var elementIds = new List<int>();
                foreach (string hexString in dynamicAvatar.SplitInParts(2))
                {
                    try
                    {
                        elementIds.Add(Convert.ToInt32(hexString, 16));
                    }
                    catch (Exception)
                    {
                        problem = true;
                        break;
                    }
                }
                if (!problem)
                {
                    avatarLayerElement = await dynamicAvatarService.ReturnValidated(elementIds);
                    if (avatarLayerElement == null)
                    {
                        problem = true;
                    }
                }
            }
            var details = new DynamicAvatarDetails
            {
                DynamicAvatarPaths = new Dictionary<int, string>()
            };
            var dynamicAvatarString = new StringBuilder();
            if (avatarLayerElement != null && !problem)
            {
                int zIndex = 1;
                int siteId = GetCurrentSiteId();
                foreach (int layerId in avatarLayerElement.Keys)
                {
                    string path = $"site{siteId}/dynamicavatars/layer{layerId}/{avatarLayerElement[layerId]}.png";
                    details.DynamicAvatarPaths.Add(zIndex, _pathResolver.ResolveContentPath(path));
                    dynamicAvatarString.Append(avatarLayerElement[layerId].ToString("x2"));
                    zIndex++;
                }
                details.DynamicAvatarString = dynamicAvatarString.ToString();
            }
            return details;
        }
    }
}
