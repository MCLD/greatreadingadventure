using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace GRA.SRP.Core.Utilities
{
    public class PageServices
    {
        private readonly Page clientPage;

        public PageServices(Page clientPage)
        {
            this.clientPage = clientPage;
        }

        public static string CurrentPath
        {
            get
            {
                string path = HttpContext.Current.Request.Path;
                int i = path.LastIndexOf('/');
                if (i > -1)
                    return path.Substring(0, i + 1);
                return string.Empty;
            }
        }

        public static string JoinHttpPath(string part1, string part2)
        {
            var result = new StringBuilder(part1.Trim());
            if (!result.ToString().EndsWith("/"))
            {
                result.Remove(result.Length - 1, 1);
            }
            return part2.StartsWith("/") ? string.Format("{0}{1}", result, part2) : string.Format("{0}/{1}", result, part2);
        }

        public static string PageName(string path)
        {
            int i = path.LastIndexOf('/');
            if (i == -1)
                return path;
            ++i;
            if (i == path.Length)
                return string.Empty;
            return path.Substring(i, path.Length - i);
        }

        public static string PageInCurrentPath(string pagePath)
        {
            return JoinHttpPath(CurrentPath, PageName(pagePath));
        }

        public int GetRequestValueAsInt(string name)
        {
            return GetRequestValueAsInt(name, -1);
        }

        public string GetRequestValueAsString(string name)
        {
            return GetRequestValueAsString(name, string.Empty);
        }

        public int GetRequestValueAsInt(string name, int defaultValue)
        {
            string s = clientPage.Request[name];
            if (s == null)
                return defaultValue;
            int result;
            if (int.TryParse(s, out result))
                return result;
            return defaultValue;
        }

        public bool GetRequestValueAsBool(string name)
        {
            return clientPage.Request[name] != null;
        }

        public bool GetRequestValueAsBool(string name, string trueValue)
        {
            string value = clientPage.Request[name];
            return (value == null || value == trueValue);
        }


        public string GetRequestValueAsString(string name, string defaultValue)
        {
            string result = clientPage.Request[name];
            if (result == null || result.Trim().Length == 0)
                return defaultValue;
            return result;
        }

        public bool GetRequestValueAsDate(string name, out DateTime date)
        {
            string dateString = clientPage.Request[name];
            if (dateString == null || dateString.Trim().Length == 0)
            {
                date = DateTime.Now;
                return false;
            }
            return DateTime.TryParse(dateString, out date);
        }


        public Guid GetRequestValueAsGuid(string name)
        {
            return GetRequestValueAsGuid(name, true);
        }

        public Guid GetRequestValueAsGuid(string name, bool required)
        {
            string value = GetRequestValueAsString(name);
            if (value.Length == 0)
            {
                if (required)
                    throw new Exception(
                        string.Format("No Guid value found in request. Key='{0}', Value='{1}', Page='{2}'",
                                      name, value, clientPage.Request.RawUrl));
                return Guid.Empty;
            }
            Guid result;
            if (!DataValidation.IsValidGuid(value, out result))
                throw new Exception(
                    string.Format("Invalid Guid parmeter. Key='{0}', Value='{1}', Page='{2}'",
                                  name, value, clientPage.Request.RawUrl));

            return result;
        }


        public int GetViewStateValueAsInt(IDictionary dictionary, string name, int defaultValue)
        {
            object value = dictionary[name];
            if (value == null)
                return defaultValue;
            return (int)value;
        }

        public string GetViewStateValueAsString(IDictionary dictionary, string name, string defaultValue)
        {
            object result = dictionary[name];
            if (result == null)
                return defaultValue;
            return result.ToString();
        }

        public string GetViewStateValueAsString(IDictionary dictionary, string name)
        {
            return GetViewStateValueAsString(dictionary, name, string.Empty);
        }

        public Guid GetViewStateValueAsGuid(IDictionary dictionary, string name)
        {
            return GetViewStateValueAsGuid(dictionary, name, true);
        }

        public Guid GetViewStateValueAsGuid(IDictionary dictionary, string name, bool required)
        {
            object value = dictionary[name];
            if (value == null)
            {
                if (!required)
                    return Guid.Empty;
                throw new Exception(
                    string.Format("No ViewState value  found. Key = '{0}', page={1}.",
                                  name, clientPage.Request.RawUrl));
            }
            try
            {
                return (Guid)value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format("Value in ViewState is not a valid Guid. Key = '{0}', Value = '{1}', page='{2}'.",
                                  name, value, clientPage.Request.RawUrl), ex);
            }
        }

        public void RedirectToErrorPage()
        {
            clientPage.Server.Transfer("/Error.aspx");
        }

        public void LogInvalidData(string dataItem, object value, string location)
        {
            //LoggingService.LogInvalidData(dataItem, value.ToString(), clientPage.Request.RawUrl, location);

            //Logger.Write(dataItem, value.ToString());
            //Logger.Write(clientPage.Request.RawUrl, location);
        }

        public void LogInvalidItemId(string entityName, Guid value, string location)
        {
            LogInvalidData(string.Format("{0}.ItemId", entityName), value, location);
        }

        public void LogInvalidItemId(Guid value, string location)
        {
            LogInvalidData("ItemId", value, location);
        }


        public void LogInvalidPageId(int value, string location)
        {
            LogInvalidData("PageId", value, location);
        }

        public void LogInvalidCity(Guid value, string location)
        {
            LogInvalidData("City.ItemId", value, location);
        }

        public void LogInvalidCity(int value, string location)
        {
            LogInvalidData("City.PageId", value, location);
        }

        public bool HasSingleItem(DataSet ds, Guid itemId, string itemTypeName, string location, string tableName)
        {
            if (ds.Tables.Contains(tableName) && ds.Tables[tableName].Rows.Count > 0)
                return true;
            LogInvalidItemId(itemTypeName, itemId, location);
            RedirectToErrorPage();
            return false;
        }

        public bool IsValidRegion(DataRow regionData, int regionId, string location)
        {
            if (regionData == null)
            {
                LogInvalidData("Region.ID", regionId, location);
                RedirectToErrorPage();
                return false;
            }
            return true;
        }

        public bool HasSingleItem(DataSet ds, Guid itemId, string itemTypeName, string location, int tableIndex)
        {
            if (ds.Tables.Count > tableIndex && ds.Tables[tableIndex].Rows.Count > 0)
                return true;
            LogInvalidItemId(itemTypeName, itemId, location);
            RedirectToErrorPage();
            return false;
        }

        public bool HasSingleItem(DataSet ds, Guid itemId, string itemTypeName, string location)
        {
            return HasSingleItem(ds, itemId, itemTypeName, location, 0);
        }

        public string GetConfigValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            return value ?? string.Empty;
        }


        //public string GetLandingPage()
        //{
        //    int authLevel = TTSecurity.GetAuthenticationLevel(clientPage.User.Identity.Name);
        //    return GetLandingPage(authLevel);
        //}

        //public string GetLandingPage(int authLevel)
        //{
        //    switch (authLevel)
        //    {
        //        case TTSecurity.AUTHLEVEL_ADMIN:
        //            return "~/secure/admin/AdminHome.aspx";
        //        case TTSecurity.AUTHLEVEL_PARTNER:
        //            return "~/secure/partner/PartnerHome.aspx";
        //        case TTSecurity.AUTHLEVEL_REPORTUSER:
        //            return "~/reports/default.aspx";
        //        default:
        //            return "~/index.aspx";
        //    }
        //}
    }
}
