﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;

namespace STG.SRP
{
    public partial class Recover : BaseSRPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) TranslateStrings(this);
        }
    }
}