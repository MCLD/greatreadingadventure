using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GRA.Logic
{
   
    public class QueryString
    {
        public string GetSystemIdString(string system)
        {
            if (string.IsNullOrWhiteSpace(system))
            {
                return null;
            }
            var systemCodes = SRP.DAL.Codes.GetAll();
            if (systemCodes != null
                && systemCodes.Tables != null
                && systemCodes.Tables.Count > 0)
            {
                // code type id = 2 means library district/system
                string query = string.Format(
                    "CTID = 2 AND Code = '{0}'",
                    system.Trim().Replace("'", "''"));
                var rows = systemCodes.Tables[0].Select(query);
                if (rows.Count() > 0)
                {
                    return rows[0]["CID"].ToString();
                }
            }
            return null;
        }

        public Tuple<string,string> GetSystemBranchIdStrings(string branch)
        {
            if(string.IsNullOrWhiteSpace(branch))
            {
                return null;
            }
            string systemIdString = null;
            string branchIdString = null;
            var branchCodes = SRP.DAL.Codes.GetAll();
            if (branchCodes != null
                && branchCodes.Tables != null
                && branchCodes.Tables.Count > 0)
            {
                // code type id = 1 means library branch
                string query = string.Format(
                    "CTID = 1 AND Code = '{0}'",
                    branch.Trim().Replace("'", "''"));
                var rows = branchCodes.Tables[0].Select(query);
                if (rows.Count() > 0)
                {
                    branchIdString = rows[0]["CID"].ToString();
                }
            }

            if (!string.IsNullOrWhiteSpace(branchIdString))
            {
                int branchId = -1;
                if (int.TryParse(branchIdString, out branchId) && branchId != -1)
                {
                    var branchObject = SRP.DAL.LibraryCrosswalk.FetchObjectByLibraryID(branchId);
                    if (branchObject != null)
                    {
                        systemIdString = branchObject.DistrictID.ToString();
                    }
                }
            }
            return new Tuple<string, string>(systemIdString, branchIdString);
        }
    }
}
