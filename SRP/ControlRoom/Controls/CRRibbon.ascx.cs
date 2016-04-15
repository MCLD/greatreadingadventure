using GRA.SRP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom {
    public partial class CRRibbon : System.Web.UI.UserControl {
        public List<RibbonPanel> Panels { get; set; }

        public void Add(RibbonPanel panel) {
            if (Panels == null) Panels = new List<RibbonPanel>();
            Panels.Add(panel);
        }

        public new bool Visible {
            get {
                return pnlRibbon.Visible;
            }
            set {
                pnlRibbon.Visible = value;
            }
        }


        protected void PageLoad(object sender, EventArgs e) {
            if (Panels == null) Panels = new List<RibbonPanel>();
            if (!IsPostBack) {

                DataBind();
            }
        }

        public new void DataBind() {
            if (Panels == null || Panels.Count == 0) { Visible = false; return; }
            rptPnl.DataSource = Panels;
            rptPnl.DataBind();
        }

        public bool IsList(int count) {
            return count <= 4;
        }

        public string GetLinks(List<RibbonLink> links) {

            if (links.Count <= 4) {
                StringBuilder slinks = new StringBuilder("<ul class=\"cntUL0\" style=\"margin-bottom: 0px\">");
                foreach (RibbonLink l in links) {
                    if (l.Url.StartsWith("http") || l.NewWindow == true)
                    {
                        slinks.AppendFormat("<li class=\"cntSqbulletedlist\"><a href=\"{0}\" target=\"_blank\">{1}</a></li>",
                        l.Url,
                        l.Name);
                    }
                    else {
                        slinks.AppendFormat("<li class=\"cntSqbulletedlist\"><a href=\"{0}\">{1}</a></li>",
                        l.Url,
                        l.Name);
                    }
                }
                slinks.Append("</ul>");
                return slinks.ToString();
            } else {

                StringBuilder slinks = new StringBuilder("&nbsp;<select name='aList' onchange='if (this.options[this.selectedIndex].value != \"\") document.location.href=this.options[this.selectedIndex].value;'><option value=''>[Make a Selection]</option>");
                foreach (RibbonLink l in links) {
                    slinks.AppendFormat("<option value='{0}'>{1}</option>",
                        l.Url,
                        l.Name);
                }
                slinks.Append("</select>");
                return slinks.ToString();
            }
        }

    }
}