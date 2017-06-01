
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Home
{
    public class AuthorizationCodeViewModel
    {
        [Required]
        [DisplayName("Authorization Code")]
        public string AuthorizationCode { get; set; }
    }
}
