using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Certificate
{
    public partial class Default : BaseSRPPage
    {
        private const string BaseMessage = "You haven't earned a certificate yet. You have <strong>{0:n0}</strong> out of a required <strong>{1:n0}</strong> points. You'll receive the certificate after earning <strong>{2:n0}</strong> more points.";
        protected DAL.Patron Patron { get; set; }
        protected bool Achiever { get; set; }
        protected string Status { get; set; }
        protected string Explanation { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsSecure = true;
            TranslateStrings(this);

            string imageUrl = "~/images/certificate-logo.png";
            string imageUrl2x = "~/images/certificate-logo@2x.png";
            if (!System.IO.File.Exists(Server.MapPath(imageUrl)))
            {
                imageUrl = "~/images/gra150.png";
                imageUrl2x = "~/images/gra300.png";
            }

            if (System.IO.File.Exists(Server.MapPath(imageUrl2x)))
            {
                CertificateLogo.Attributes.Add("srcset", string.Format("{0} 1x, {1} 2x",
                    Page.ResolveUrl(imageUrl),
                    Page.ResolveUrl(imageUrl2x)
                    ));
            }
            CertificateLogo.Src = imageUrl;

            Achiever = false;
            Patron = Session[SessionKey.Patron] as DAL.Patron;
            if (Patron == null)
            {
                Response.Redirect("~");
            }

            var program = DAL.Programs.FetchObject(Patron.ProgID);

            if (program == null
                || program.IsActive == false
                || program.IsHidden == true
                || program.IsOpen == false
                || program.CompletionPoints == 0)
            {
                this.Log().Error($"Could not display certificate for program {program.AdminName} (id {program.PID}): Active = {program.IsActive}, Hidden = {program.IsHidden}, Open = {program.IsOpen}, Completion Points = {program.CompletionPoints}");
                Response.Redirect("~");
            }

            int points = DAL.PatronPoints.GetTotalPatronPoints(Patron.PID);

            if (points >= program.CompletionPoints)
            {
                Achiever = true;
            }
            else
            {
                Status = string.Format("{0:0}%", points * 100 / program.CompletionPoints);
                progressBar.Style.Add("width", Status);
                string message = StringResources.getStringOrNull("certificate-not-yet");
                if (string.IsNullOrEmpty(message))
                {
                    message = BaseMessage;
                }
                if (message.Contains("{2"))
                {
                    try
                    {
                        Explanation = string.Format(message,
                            points,
                            program.CompletionPoints,
                            program.CompletionPoints - points);
                    }
                    catch (Exception ex)
                    {
                        this.Log().Error($"Error formatting certificate-not-yet: {ex.Message} - {ex.StackTrace}");
                        Explanation = string.Format(BaseMessage,
                            points,
                            program.CompletionPoints,
                            program.CompletionPoints - points);
                    }
                }
                else
                {
                    Explanation = message;
                }
            }

            var masterPage = this.Master as SRP.SRPMaster;
            if (masterPage != null)
            {
                var printHeader = masterPage.FindControl("printHeader");
                if (printHeader != null)
                {
                    printHeader.Visible = false;
                }
            }
        }
    }
}