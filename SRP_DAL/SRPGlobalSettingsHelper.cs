using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;

namespace SRPApp.Classes
{
[Serializable]    public class SRPGlobalSettingsHelper : EntityBase
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        //public static string GetSetting(string name)
        //{
        //    if (HttpContext.Current.Application[name] != null)
        //    {
        //        return HttpContext.Current.Application[name].ToString();

        //    }
        //    SRPGlobalSettingsHelper s = GetByName(name);
        //    string setting = s.Value;

        //    HttpContext.Current.Application[name] = setting;
        //    return setting;
        //}

        //public static string[] GetSetting(string name, string delimiter)
        //{
        //    string setting = GetSetting(name);
        //    if (string.IsNullOrEmpty(setting))
        //        return new string[] { };
        //    string[] result = new string[] { };
        //    if (!string.IsNullOrEmpty(setting))
        //        result = setting.Split(delimiter.ToCharArray());
        //    return result;
        //}

        //public static void SetSetting(string name, string value)
        //{
        //    HttpContext.Current.Application[name] = value;
        //    // save to database here
        //    SRPGlobalSettingsHelper s = GetByName(name);
        //    s.Value = value;
        //    Update(s);
        //}

        //public static void SetSetting(string name, string[] value, string delimiter)
        //{
        //    string setting = "";
        //    foreach (var v in value)
        //    {
        //        setting = setting + delimiter.Trim() + v.Trim();
        //    }
        //    if (!string.IsNullOrEmpty(setting))
        //        setting = setting.Remove(0, delimiter.Trim().Length);
        //    SetSetting(name, setting);
        //}


        public static DataTable GetAllByModuleAsDataTable(int modId)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@modId", modId);

            return SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSGlobalModuleSetting where ModuleId = @modId", arrParams).Tables[0];
        }

        #region Entity Columns

        public int UID { get; set; }
        public int ModuleID { get; set; }
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public string Type { get; set; }
        public string Editor { get; set; }
        public string ListValues { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        #endregion		

        public static SRPGlobalSettingsHelper GetByPrimaryKey(int uid)
        {
            var arrParams = new SqlParameter[1];

            arrParams[0] = new SqlParameter("@UID", uid);

            var dt =
                SqlHelper.ExecuteDataset(conn, CommandType.Text, "Select * from CMSGlobalModuleSetting where UID = @UID",
                                         arrParams).Tables[0];
            SRPGlobalSettingsHelper[] tempArray = MapRecords(dt);
            return 0 == tempArray.Length ? null : tempArray[0];

        }

        protected static SRPGlobalSettingsHelper[] MapRecords(DataTable dt)
        {
            var recordList = new System.Collections.ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                var s = new SRPGlobalSettingsHelper();
                s.UID = (int)r["UID"];
                s.ModuleID = (int)r["ModuleID"];
                s.ListValues =r["ListValues"].ToString();
                s.Description = r["Name"].ToString();
                s.Name = r["Name"].ToString();
                s.PropertyName = r["PropertyName"].ToString();
                s.Type = r["Type"].ToString();
                s.Editor = r["Editor"].ToString();
                s.Value = r["Value"].ToString();
                recordList.Add(s);
            }
            return (SRPGlobalSettingsHelper[])(recordList.ToArray(typeof(SRPGlobalSettingsHelper)));
        }


        public virtual bool Update()
        {
            var arrParams = new SqlParameter[6];
            arrParams[0] = new SqlParameter("@UID", UID);
            arrParams[1] = new SqlParameter("@Name", Name);
            arrParams[2] = new SqlParameter("@Value", Value);
            arrParams[3] = new SqlParameter("@ModuleID", ModuleID);
            arrParams[4] = new SqlParameter("@PropertyName", PropertyName);
            arrParams[5] = new SqlParameter("@ListValues", ListValues);
            arrParams[5] = new SqlParameter("@Type", Type);
            arrParams[5] = new SqlParameter("@Editor", Editor);
            SqlHelper.ExecuteScalar(conn, CommandType.Text, "Update CMSGlobalModuleSetting " +
                "set Name=@Name, Value=@Value, ModuleID=@ModuleID, PropertyName=@PropertyName, ListValues=@ListValues,  Type= @Type, Editor=@Editor "+
                " where UID = @UID", arrParams);
            return true;
        }


    }
}