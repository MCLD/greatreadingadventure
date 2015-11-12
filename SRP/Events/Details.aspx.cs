using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GRA.SRP.Events {
    public partial class Details : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            TranslateStrings(this);

            if(Request.UrlReferrer == null) {
                eventBackLink.NavigateUrl = "~/Events/";
            } else {
                eventBackLink.NavigateUrl = Request.UrlReferrer.AbsolutePath;
            }

            DAL.Event evnt = null;
            int eventId = 0;
            string displayEvent = Request.QueryString["EventId"];
            if(!string.IsNullOrEmpty(displayEvent)
               && int.TryParse(displayEvent.ToString(), out eventId)) {
                evnt = DAL.Event.GetEvent(eventId);
                if(evnt != null) {
                    eventTitle.Text = evnt.EventTitle;
                    eventWhen.Text = DAL.Event.DisplayEventDateTime(evnt);
                    if(evnt.BranchID > 0) {
                        var codeObject = DAL.Codes.FetchObject(evnt.BranchID);
                        if(codeObject != null) {
                            eventWhere.Text = codeObject.Code;
                        }
                    }
                    if(string.IsNullOrWhiteSpace(eventWhere.Text)) {
                        eventWhere.Visible = false;
                        atLabel.Visible = false;
                    } else {
                        eventWhere.Visible = true;
                        atLabel.Visible = true;
                    }
                    eventShortDescription.Text = evnt.ShortDescription;
                    eventDescription.Text = evnt.HTML;
                    var cf = DAL.CustomEventFields.FetchObject();
                    if(!string.IsNullOrWhiteSpace(evnt.Custom1)
                       && !string.IsNullOrWhiteSpace(cf.Label1)) {
                        eventCustom1Panel.Visible = true;
                        eventCustomLabel1.Text = cf.Label1;
                        eventCustomValue1.Text = evnt.Custom1;
                    } else {
                        eventCustom1Panel.Visible = false;
                    }
                    if(!string.IsNullOrWhiteSpace(evnt.Custom2)
                       && !string.IsNullOrWhiteSpace(cf.Label2)) {
                        eventCustom2Panel.Visible = true;
                        eventCustomLabel2.Text = cf.Label2;
                        eventCustomValue2.Text = evnt.Custom2;
                    } else {
                        eventCustom2Panel.Visible = false;
                    }
                    if(!string.IsNullOrWhiteSpace(evnt.Custom3)
                       && !string.IsNullOrWhiteSpace(cf.Label3)) {
                        eventCustom3Panel.Visible = true;
                        eventCustomLabel3.Text = cf.Label3;
                        eventCustomValue3.Text = evnt.Custom3;
                    } else {
                        eventCustom3Panel.Visible = false;
                    }
                    eventDetails.Visible = true;
                }
            }

            if(evnt == null) {
                eventDetails.Visible = false;
                var cph = Page.Master.FindControl("HeaderContent") as ContentPlaceHolder;
                if(cph != null) {
                    cph.Controls.Add(new HtmlMeta {
                        Name = "robots",
                        Content = "noindex"
                    });
                }
                Session[SessionKey.PatronMessage] = "Could not find details on that event.";
                Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Danger;
                Session[SessionKey.PatronMessageGlyphicon] = "remove";

            }
        }
    }
}