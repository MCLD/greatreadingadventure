using System;
using System.Data;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class DashboardStats : BaseControlRoomPage
    {
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true; 
            
            if (!IsPostBack)
            {
                MasterPage.PageTitle = string.Format("{0}", "Statistics Dashboard");

                SetPageRibbon(StandardModuleRibbons.ReportsRibbon());

                var arrParams = new SqlParameter[1];
                arrParams[0] = new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID);

                var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_DashboardStats", arrParams);

                GetRegByProg(ds.Tables[0]);
                GetFinisherByProg(ds.Tables[1]);

                GetRegAgeByProgLabels(ds.Tables[2]);
                GetRegAgeByProg(ds.Tables[3]);
                
                GetFinisherAgeByProgLabels(ds.Tables[4]);
                GetFinisherAgeByProg(ds.Tables[5]);

                GetRegGenderByProg(ds.Tables[6]);
                GetFinisherGenderByProg(ds.Tables[7]);
            }
        }

        public void GetRegByProg(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i<dt.Rows.Count; i++)
                {
                    retStr = Coalesce(retStr, BuildPieDataPoint(dt.Rows[i]["Program"].ToString(), dt.Rows[i]["RegistrantCount"].ToString()), ",");
                }
            }

            ViewState["Pie1"] = retStr;
        }

        public void GetFinisherByProg(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    retStr = Coalesce(retStr, BuildPieDataPoint(dt.Rows[i]["Program"].ToString(), dt.Rows[i]["FinisherCount"].ToString()), ",");
                }
            }

            ViewState["Pie2"] = retStr;
        }

        public void GetRegAgeByProgLabels(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    retStr = Coalesce(retStr, "'" + dt.Rows[i]["Label"].ToString() + "'", ",");
                }
            }

            ViewState["Labels1"] = retStr;

        }

         public void GetFinisherAgeByProgLabels(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    retStr = Coalesce(retStr, "'" + dt.Rows[i]["Label"].ToString() + "'", ",");
                }
            }

            ViewState["Labels2"] = retStr;

        }

        public void GetRegAgeByProg(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var rowStr = "['Age ";
                    for (int j = 0; j < dt.Columns.Count;  j++)
                    {
                        if (j == 0)
                        {
                            rowStr = rowStr + dt.Rows[i][j].ToString() + "',";
                        }
                        else
                        {
                            rowStr = rowStr + dt.Rows[i][j].ToString() + (j == dt.Columns.Count-1 ? "" : ",");
                        }
                    }
                    rowStr = rowStr + "]";
                    retStr = Coalesce(retStr, rowStr, ",");
                }
            }

            ViewState["Data1"] = retStr;
        }

        public void GetFinisherAgeByProg(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var rowStr = "['Age ";
                    for (int j = 0; j < dt.Columns.Count;  j++)
                    {
                        if (j == 0)
                        {
                            rowStr = rowStr + dt.Rows[i][j].ToString() + "',";
                        }
                        else
                        {
                            rowStr = rowStr + dt.Rows[i][j].ToString() + (j == dt.Columns.Count-1 ? "" : ",");
                        }
                    }
                    rowStr = rowStr + "]";
                    retStr = Coalesce(retStr, rowStr, ",");
                }
            }

            ViewState["Data2"] = retStr;
        }

        public void GetRegGenderByProg(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    retStr = Coalesce(retStr, 
                            Build5DataPoint(dt.Rows[i]["AdminName"].ToString(), 
                                            dt.Rows[i]["MaleRegistrant"].ToString(),
                                            dt.Rows[i]["FemaleRegistrant"].ToString(),
                                            dt.Rows[i]["OtherRegistrant"].ToString(),
                                            dt.Rows[i]["NARegistrant"].ToString()
                            )
                            , ",");
                }
            }

            ViewState["Stacked1"] = retStr;
        }

        public void GetFinisherGenderByProg(DataTable dt)
        {
            var retStr= string.Empty;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    retStr = Coalesce(retStr,
                            Build5DataPoint(dt.Rows[i]["AdminName"].ToString(),
                                            dt.Rows[i]["MaleFinisher"].ToString(),
                                            dt.Rows[i]["FemaleFinisher"].ToString(),
                                            dt.Rows[i]["OtherFinisher"].ToString(),
                                            dt.Rows[i]["NAFinisher"].ToString()
                            )
                            , ",");
                }
            }

            ViewState["Stacked2"] = retStr;
        }

        private string Coalesce(string startingString, string additionalString, string separator= "")
        {
            if (startingString.Length == 0) return additionalString;
            return string.Format("{0} {1} {2}", startingString, separator, additionalString);
        }

        private string BuildPieDataPoint (string str1, string str2)
        {
            return string.Format("['{0}', {1}]", str1, str2);
        }
        private string Build4DataPoint(string str1, string str2, string str3, string str4)
        {
            return string.Format("['{0}', {1}, {2}, {3}]", str1, str2, str3, str4);
        }

        private string Build5DataPoint(string str1, string str2, string str3, string str4, string str5)
        {
            return string.Format("['{0}', {1}, {2}, {3}, {4}]", str1, str2, str3, str4, str5);
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            var rs = GetReportData();

        }

        private DataSet GetReportData()
        {
            var arrParams = new SqlParameter[6];

            if (ProgID.SelectedValue == "0")
            {
                arrParams[0] = new SqlParameter("@ProgId", DBNull.Value);
            }
            else
            {
                arrParams[0] = new SqlParameter("@ProgId", ProgID.SelectedValue);
            }
            if (BranchID.SelectedValue == "0")
            {
                arrParams[1] = new SqlParameter("@BranchID", DBNull.Value);
            }
            else
            {
                arrParams[1] = new SqlParameter("@BranchID", BranchID.SelectedValue);
            }
            if (LibSys.SelectedValue == "")
            {
                arrParams[2] = new SqlParameter("@LibSys", DBNull.Value);
            }
            else
            {
                arrParams[2] = new SqlParameter("@LibSys", LibSys.SelectedValue);
            }
            if (School.SelectedValue == "")
            {
                arrParams[3] = new SqlParameter("@School", DBNull.Value);
            }
            else
            {
                arrParams[3] = new SqlParameter("@School", School.SelectedValue);
            }
            if (Level.Text  == "")
            {
                arrParams[4] = new SqlParameter("@Level", DBNull.Value);
            }
            else
            {
                arrParams[4] = new SqlParameter("@Level", int.Parse( Level.Text));
            }
            arrParams[5] = new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID);

            //var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_FinisherStats", arrParams);

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_DashboardStats", arrParams);

            GetRegByProg(ds.Tables[0]);
            GetFinisherByProg(ds.Tables[1]);

            GetRegAgeByProgLabels(ds.Tables[2]);
            GetRegAgeByProg(ds.Tables[3]);

            GetFinisherAgeByProgLabels(ds.Tables[4]);
            GetFinisherAgeByProg(ds.Tables[5]);

            GetRegGenderByProg(ds.Tables[6]);
            GetFinisherGenderByProg(ds.Tables[7]);

            return ds;
        }


        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgID.SelectedValue = BranchID.SelectedValue = "0";
            School.SelectedValue = LibSys.SelectedValue= string.Empty;

            btnFilter_Click(sender, e);
        }

    }
}
