using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities {
    public class RibbonLink {
        #region Fields (2)

        private string _name;
        private string _url;

        #endregion Fields

        #region Properties (2)

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

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