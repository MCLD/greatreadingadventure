using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Configuration;

namespace STG.SRP.Portal
{
    public class BasePage : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            System.Configuration.ConnectionStringSettings _connectionSettings = null;
            _connectionSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            IDbConnection _connection = null;
            if (_connectionSettings.ProviderName.ToLower().Contains("sql"))
            { _connection = new SqlConnection(_connectionSettings.ConnectionString); }
            else if (_connectionSettings.ProviderName.ToLower().Contains("ole"))
            { _connection = new OleDbConnection(_connectionSettings.ConnectionString); }
            else if (_connectionSettings.ProviderName.ToLower().Contains("odbc"))
            { _connection = new OdbcConnection(_connectionSettings.ConnectionString); }
            else
            { _connection = new SqlConnection(_connectionSettings.ConnectionString); }
            IDbCommand dbCmd = _connection.CreateCommand();

            //foreach (IDbDataParameter p in parameters)
            //{ dbCmd.Parameters.Add(p); }

            dbCmd.CommandText = "getMaster";
            dbCmd.CommandType = CommandType.StoredProcedure;

            IDbDataAdapter adapter = null;

            //Inspect the provider type and create an instance of the matching connection object.
            //Use Activator.CreateInstance for easy management.
            //If performance becomes an issue, replace this line with hard coded direct object type creation code.
            if (_connectionSettings.ProviderName.ToLower().Contains("sql"))
            { adapter = new SqlDataAdapter(); }
            else if (_connectionSettings.ProviderName.ToLower().Contains("ole"))
            { adapter = new OleDbDataAdapter(); }
            else if (_connectionSettings.ProviderName.ToLower().Contains("odbc"))
            { adapter = new OdbcDataAdapter(); }
            else
            { adapter = new SqlDataAdapter(); }

            adapter.SelectCommand = dbCmd;

            DataSet ds = new DataSet();
            _connection.Open();
            adapter.Fill(ds);
            _connection.Close();

            string TemplateName = ds.Tables[0].Rows[0][0].ToString();
            Page.MasterPageFile = TemplateName;


//            Page.MasterPageFile = "~/Templates/AlternateSitetemplate.Master";

            MasterPage MP = this.Master;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // the list of content placeholders is defined and fixed
            // for each ContentPlaceHolder
            // do this

            // Controls need to be loaded on every page load
            Control MyC = LoadControl("~/Controls/Greeter.ascx");
            // Dynamic - get the property and set the value
            // so that the page load of the control can behave differently
            if (!IsPostBack)
            {
                PropertyInfo propInfo = MyC.GetType().GetProperty("InitName");
                propInfo.SetValue(MyC, "BJ", null);
            }

            // the placeholder will always have a Placeholder control 
            //      named "ContentPlaceHolderID" + "_ph"
            // Render the control on the page
            ContentPlaceHolder mainContent = (ContentPlaceHolder)this.Master.FindControl("CPH1");
            PlaceHolder ph = (PlaceHolder)mainContent.FindControl("CPH1_ph");

            //PlaceHolder ph = (PlaceHolder)FindControl("CPH1_ph");
            ph.Controls.Add(MyC);
            //ph.Controls.Add(MyC);

            // simple dynamic control
            Control MyC2 = LoadControl("~/Controls/Greeter.ascx");
            ph.Controls.Add(MyC2);
            //ph.Controls.Add(MyC2);

        }
    }
}
