using GRA.Controllers.Filter;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(MissionControlFilter), Order = 2)]
    public abstract class MCController : Controller
    {
        protected static readonly string[] ValidUploadExtensions
            = { ".jpeg", ".jpg", ".pdf", ".png" };

        protected static readonly string[] ValidCsvExtensions = { ".csv" };

        protected MCController(ServiceFacade.Controller context) : base(context)
        {
        }

        protected string GetBadgeMakerUrl(string origin, string email)
        {
            return $"https://www.openbadges.me/designer.html?origin={origin}&email={email}";
        }
    }
}
