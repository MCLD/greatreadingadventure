using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities {
    public class RibbonLink {
        #region Properties (2)
        public string Name { get; set; }
        public string Url { get; set; }
        public bool NewWindow { get; set; }
        #endregion Properties 
    }

    public class RibbonPanel {
        #region Constructors (1)

        public RibbonPanel() {
            this.ImagePath = "/ControlRoom/Images/ZA101776221033.gif";
            this.ImageAlt = "N/A";
            this.Name = "Default Ribbon Panel";
            this.Links = new List<RibbonLink>();
        }

        #endregion Constructors

        #region Properties (4)

        public string ImageAlt { get; set; }
        public string ImagePath { get; set; }
        public string ImagePath2x { get; set; }
        public List<RibbonLink> Links { get; set; }
        public string Name { get; set; }

        #endregion Properties

        #region Methods (2)

        // Public Methods (2) 

        public void Add(RibbonLink link) {
            Links.Add(link);
        }

        public void Add(string pName, string pUrl) {
            Links.Add(new RibbonLink { Name = pName, Url = pUrl });
        }
        #endregion Methods
    }

}