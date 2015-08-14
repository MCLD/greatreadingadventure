using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities
{
    public class SRPTopMenuItem
    {
        #region Fields (3)

        private bool _isSelected;
        private string _name;
        private string _url;

        #endregion Fields

        #region Properties (3)

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

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
}
