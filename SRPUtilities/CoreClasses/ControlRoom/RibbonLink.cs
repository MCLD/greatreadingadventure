using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities
{
    public class RibbonLink
    {
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

    public class RibbonPanel
    {
        #region Fields (4)

        private string _imageAlt;
        private string _imagePath;
        private List<RibbonLink> _links;
        private string _name;

        #endregion Fields

        #region Constructors (1)

        public RibbonPanel()
        {
            ImagePath = "/ControlRoom/Images/ZA101776221033.gif";
            ImageAlt = "N/A";
            Name = "Default Ribbon Panel";
            Links = new List<RibbonLink>();
        }

        #endregion Constructors

        #region Properties (4)

        public string ImageAlt
        {
            get { return _imageAlt; }
            set { _imageAlt = value; }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        public List<RibbonLink> Links
        {
            get { return _links; }
            set { _links = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion Properties

        #region Methods (2)

        // Public Methods (2) 

        public void Add(RibbonLink link)
        {
            Links.Add(link);
        }

        public void Add(string pName, string pUrl)
        {
            Links.Add(new RibbonLink { Name = pName, Url = pUrl });
        }

        #endregion Methods
    }

}