using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Utilities
{
    public class SRPPermission
    {
        #region Fields (3)

        private string _desc;
        private string _name;
        private int _permId;

        #endregion Fields

        #region Properties (3)

        public string Description
        {
            get { return _desc; }
            set { _desc = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Permission
        {
            get { return _permId; }
            set { _permId = value; }
        }

        #endregion Properties
    }
}
