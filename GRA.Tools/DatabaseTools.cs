using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRA.Tools
{
    public class DatabaseTools
    {
        public string PrepareSearchString(string searchText)
        {
            if(searchText.Length > 255)
            {
                searchText = searchText.Substring(0, 255);
            }
            return string.Format("%{0}%", searchText.Replace("%", string.Empty));
        }
    }
}
