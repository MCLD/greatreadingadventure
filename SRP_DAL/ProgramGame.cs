using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.ApplicationBlocks.Data;
using System.Collections;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.DAL
{
    [Serializable]
	public class ProgramGame : EntityBase
	{
        public static new string Version { get { return "2.0"; } }

			#region Private Variables

			private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

			private int myPGID;
			private string myGameName = "";
            private string myMapImage = "";
            private string myBonusMapImage = "";
			private int myBoardWidth = 0;
			private int myBoardHeight = 0;
			private decimal myBonusLevelPointMultiplier=1;
            private string myLevelCompleteImage = "";
			private string myLastModUser;
			private DateTime myAddedDate;
			private string myAddedUser;
			private DateTime myLastModDate;

            private int myTenID = 0;
            private int myFldInt1 = 0;
            private int myFldInt2 = 0;
            private int myFldInt3 = 0;
            private bool myFldBit1 = false;
            private bool myFldBit2 = false;
            private bool myFldBit3 = false;
            private string myFldText1 = "";
            private string myFldText2 = "";
            private string myFldText3 = "";

            private int myMinigame1ID = 0;
            private int myMinigame2ID = 0;
			#endregion

			#region Accessors

public int PGID
{
	get { return myPGID; }
	set { myPGID = value; }
}
public string GameName
{
	get { return myGameName; }
	set { myGameName = value; }
}
public string MapImage
{
	get { return myMapImage; }
	set { myMapImage = value; }
}
public string BonusMapImage
{
	get { return myBonusMapImage; }
	set { myBonusMapImage = value; }
}
public int BoardWidth
{
	get { return myBoardWidth; }
	set { myBoardWidth = value; }
}
public int BoardHeight
{
	get { return myBoardHeight; }
	set { myBoardHeight = value; }
}
public decimal BonusLevelPointMultiplier
{
	get { return myBonusLevelPointMultiplier; }
	set { myBonusLevelPointMultiplier = value; }
}
public string LevelCompleteImage
{
	get { return myLevelCompleteImage; }
	set { myLevelCompleteImage = value; }
}
public string LastModUser
{
	get { return myLastModUser; }
	set { myLastModUser = value; }
}
public DateTime AddedDate
{
	get { return myAddedDate; }
	set { myAddedDate = value; }
}
public string AddedUser
{
	get { return myAddedUser; }
	set { myAddedUser = value; }
}
public DateTime LastModDate
{
	get { return myLastModDate; }
	set { myLastModDate = value; }
}
public int TenID
{
    get { return myTenID; }
    set { myTenID = value; }
}

public int FldInt1
{
    get { return myFldInt1; }
    set { myFldInt1 = value; }
}

public int FldInt2
{
    get { return myFldInt2; }
    set { myFldInt2 = value; }
}

public int FldInt3
{
    get { return myFldInt3; }
    set { myFldInt3 = value; }
}

public bool FldBit1
{
    get { return myFldBit1; }
    set { myFldBit1 = value; }
}

public bool FldBit2
{
    get { return myFldBit2; }
    set { myFldBit2 = value; }
}

public bool FldBit3
{
    get { return myFldBit3; }
    set { myFldBit3 = value; }
}

public string FldText1
{
    get { return myFldText1; }
    set { myFldText1 = value; }
}

public string FldText2
{
    get { return myFldText2; }
    set { myFldText2 = value; }
}

public string FldText3
{
    get { return myFldText3; }
    set { myFldText3 = value; }
}

        public int Minigame1ID
        {
            get { return myMinigame1ID; }
            set { myMinigame1ID = value; }
        }
        public int Minigame2ID
        {
            get { return myMinigame2ID; }
            set { myMinigame2ID = value; }
        }

			#endregion

			#region Constructors

            public ProgramGame()
            {
                TenID = (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ? -1 : (int)HttpContext.Current.Session["TenantID"]);

            }

			#endregion

		#region stored procedure wrappers

	public static DataSet GetAll() 
	{
        var arrParams = new SqlParameter[1];
        arrParams[0] = new SqlParameter("@TenID",
                            (HttpContext.Current.Session["TenantID"] == null || HttpContext.Current.Session["TenantID"].ToString() == "" ?
                                    -1 :
                                    (int)HttpContext.Current.Session["TenantID"])
                        );
        return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramGame_GetAll", arrParams);
	}

    public static DataSet GetAll(int TenID)
    {
        var arrParams = new SqlParameter[1];
        arrParams[0] = new SqlParameter("@TenID", TenID);
        return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_ProgramGame_GetAll", arrParams);
    }
    
        public static ProgramGame FetchObject(int PGID) 
	{

		// declare reader

		SqlDataReader dr;

		SqlParameter[] arrParams = new SqlParameter[1];

		arrParams[0] = new SqlParameter("@PGID", PGID);

		dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGame_GetByID", arrParams);

		if(dr.Read()) 
		{

			 // declare return value

			ProgramGame result = new ProgramGame();

			DateTime _datetime;

			int _int;

			decimal _decimal;

			if (int.TryParse(dr["PGID"].ToString(), out _int)) result.PGID = _int;
			result.GameName = dr["GameName"].ToString();
			result.MapImage = dr["MapImage"].ToString();
			result.BonusMapImage = dr["BonusMapImage"].ToString();
			if (int.TryParse(dr["BoardWidth"].ToString(), out _int)) result.BoardWidth = _int;
			if (int.TryParse(dr["BoardHeight"].ToString(), out _int)) result.BoardHeight = _int;
            if (decimal.TryParse(dr["BonusLevelPointMultiplier"].ToString(), out _decimal)) result.BonusLevelPointMultiplier = _decimal;
			result.LevelCompleteImage = dr["LevelCompleteImage"].ToString();
			result.LastModUser = dr["LastModUser"].ToString();
			if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) result.AddedDate = _datetime;
			result.AddedUser = dr["AddedUser"].ToString();
			if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) result.LastModDate = _datetime;

            if (int.TryParse(dr["TenID"].ToString(), out _int)) result.TenID = _int;
            if (int.TryParse(dr["FldInt1"].ToString(), out _int)) result.FldInt1 = _int;
            if (int.TryParse(dr["FldInt2"].ToString(), out _int)) result.FldInt2 = _int;
            if (int.TryParse(dr["FldInt3"].ToString(), out _int)) result.FldInt3 = _int;
            result.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
            result.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
            result.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
            result.FldText1 = dr["FldText1"].ToString();
            result.FldText2 = dr["FldText2"].ToString();
            result.FldText3 = dr["FldText3"].ToString();

            if (int.TryParse(dr["Minigame1ID"].ToString(), out _int)) result.Minigame1ID = _int;
            if (int.TryParse(dr["Minigame2ID"].ToString(), out _int)) result.Minigame2ID = _int;

			dr.Close();

			return result;

		}

		dr.Close();

		return null;

	}

	public bool Fetch(int PGID) 
	{

		// declare reader

		SqlDataReader dr;

		SqlParameter[] arrParams = new SqlParameter[1];

		arrParams[0] = new SqlParameter("@PGID", PGID);

		dr = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "app_ProgramGame_GetByID", arrParams);

		if(dr.Read()) 
		{

			 // declare return value

			ProgramGame result = new ProgramGame();

			DateTime _datetime;

			int _int;

			decimal _decimal;

			if (int.TryParse(dr["PGID"].ToString(), out _int)) this.PGID = _int;
			this.GameName = dr["GameName"].ToString();
			this.MapImage = dr["MapImage"].ToString();
			this.BonusMapImage = dr["BonusMapImage"].ToString();
			if (int.TryParse(dr["BoardWidth"].ToString(), out _int)) this.BoardWidth = _int;
			if (int.TryParse(dr["BoardHeight"].ToString(), out _int)) this.BoardHeight = _int;
            if (decimal.TryParse(dr["BonusLevelPointMultiplier"].ToString(), out _decimal)) this.BonusLevelPointMultiplier = _decimal;
			this.LevelCompleteImage = dr["LevelCompleteImage"].ToString();
			this.LastModUser = dr["LastModUser"].ToString();
			if (DateTime.TryParse(dr["AddedDate"].ToString(), out _datetime)) this.AddedDate = _datetime;
			this.AddedUser = dr["AddedUser"].ToString();
			if (DateTime.TryParse(dr["LastModDate"].ToString(), out _datetime)) this.LastModDate = _datetime;

            if (int.TryParse(dr["TenID"].ToString(), out _int)) this.TenID = _int;
            if (int.TryParse(dr["FldInt1"].ToString(), out _int)) this.FldInt1 = _int;
            if (int.TryParse(dr["FldInt2"].ToString(), out _int)) this.FldInt2 = _int;
            if (int.TryParse(dr["FldInt3"].ToString(), out _int)) this.FldInt3 = _int;
            this.FldBit1 = bool.Parse(dr["FldBit1"].ToString());
            this.FldBit2 = bool.Parse(dr["FldBit2"].ToString());
            this.FldBit3 = bool.Parse(dr["FldBit3"].ToString());
            this.FldText1 = dr["FldText1"].ToString();
            this.FldText2 = dr["FldText2"].ToString();
            this.FldText3 = dr["FldText3"].ToString();

            if (int.TryParse(dr["Minigame1ID"].ToString(), out _int)) this.Minigame1ID = _int;
            if (int.TryParse(dr["Minigame2ID"].ToString(), out _int)) this.Minigame2ID = _int;

			dr.Close();

			return true;

		}

		dr.Close();

		return false;

	}

	public int Insert()
	{

		return Insert(this); 

	}

	public static int Insert(ProgramGame o )
	{

		SqlParameter[] arrParams = new SqlParameter[24];

		arrParams[0]  = new SqlParameter("@GameName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameName, o.GameName.GetTypeCode()));
		arrParams[1]  = new SqlParameter("@MapImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MapImage, o.MapImage.GetTypeCode()));
		arrParams[2]  = new SqlParameter("@BonusMapImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BonusMapImage, o.BonusMapImage.GetTypeCode()));
		arrParams[3]  = new SqlParameter("@BoardWidth", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BoardWidth, o.BoardWidth.GetTypeCode()));
		arrParams[4]  = new SqlParameter("@BoardHeight", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BoardHeight, o.BoardHeight.GetTypeCode()));
		arrParams[5]  = new SqlParameter("@BonusLevelPointMultiplier", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BonusLevelPointMultiplier, o.BonusLevelPointMultiplier.GetTypeCode()));
		arrParams[6]  = new SqlParameter("@LevelCompleteImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LevelCompleteImage, o.LevelCompleteImage.GetTypeCode()));
		arrParams[7]  = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
		arrParams[8]  = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
		arrParams[9]  = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
		arrParams[10]  = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));

        arrParams[11] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
        arrParams[12] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
        arrParams[13] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
        arrParams[14] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
        arrParams[15] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
        arrParams[16] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
        arrParams[17] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
        arrParams[18] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
        arrParams[19] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
        arrParams[20] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

        arrParams[21] = new SqlParameter("@Minigame1ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame1ID, o.Minigame1ID.GetTypeCode()));
        arrParams[22] = new SqlParameter("@Minigame2ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame2ID, o.Minigame2ID.GetTypeCode()));

        arrParams[23]  = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));
		arrParams[23].Direction = ParameterDirection.Output;

		SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGame_Insert", arrParams);

		o.PGID = int.Parse(arrParams[23].Value.ToString());

		return o.PGID;

	}

	public int Update()
	{

		return Update(this); 

	}

	public static int Update(ProgramGame o )
	{

		int iReturn = -1; //assume the worst

		SqlParameter[] arrParams = new SqlParameter[24];

		arrParams[0]  = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));
		arrParams[1]  = new SqlParameter("@GameName", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.GameName, o.GameName.GetTypeCode()));
		arrParams[2]  = new SqlParameter("@MapImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.MapImage, o.MapImage.GetTypeCode()));
		arrParams[3]  = new SqlParameter("@BonusMapImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BonusMapImage, o.BonusMapImage.GetTypeCode()));
		arrParams[4]  = new SqlParameter("@BoardWidth", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BoardWidth, o.BoardWidth.GetTypeCode()));
		arrParams[5]  = new SqlParameter("@BoardHeight", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BoardHeight, o.BoardHeight.GetTypeCode()));
		arrParams[6]  = new SqlParameter("@BonusLevelPointMultiplier", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.BonusLevelPointMultiplier, o.BonusLevelPointMultiplier.GetTypeCode()));
		arrParams[7]  = new SqlParameter("@LevelCompleteImage", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LevelCompleteImage, o.LevelCompleteImage.GetTypeCode()));
		arrParams[8]  = new SqlParameter("@LastModUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModUser, o.LastModUser.GetTypeCode()));
		arrParams[9]  = new SqlParameter("@AddedDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedDate, o.AddedDate.GetTypeCode()));
		arrParams[10]  = new SqlParameter("@AddedUser", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.AddedUser, o.AddedUser.GetTypeCode()));
		arrParams[11]  = new SqlParameter("@LastModDate", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.LastModDate, o.LastModDate.GetTypeCode()));

        arrParams[12] = new SqlParameter("@TenID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.TenID, o.TenID.GetTypeCode()));
        arrParams[13] = new SqlParameter("@FldInt1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt1, o.FldInt1.GetTypeCode()));
        arrParams[14] = new SqlParameter("@FldInt2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt2, o.FldInt2.GetTypeCode()));
        arrParams[15] = new SqlParameter("@FldInt3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldInt3, o.FldInt3.GetTypeCode()));
        arrParams[16] = new SqlParameter("@FldBit1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit1, o.FldBit1.GetTypeCode()));
        arrParams[17] = new SqlParameter("@FldBit2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit2, o.FldBit2.GetTypeCode()));
        arrParams[18] = new SqlParameter("@FldBit3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldBit3, o.FldBit3.GetTypeCode()));
        arrParams[19] = new SqlParameter("@FldText1", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText1, o.FldText1.GetTypeCode()));
        arrParams[20] = new SqlParameter("@FldText2", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText2, o.FldText2.GetTypeCode()));
        arrParams[21] = new SqlParameter("@FldText3", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.FldText3, o.FldText3.GetTypeCode()));

        arrParams[22] = new SqlParameter("@Minigame1ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame1ID, o.Minigame1ID.GetTypeCode()));
        arrParams[23] = new SqlParameter("@Minigame2ID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.Minigame2ID, o.Minigame2ID.GetTypeCode()));

		try

		{
			iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGame_Update", arrParams);
		}
		catch(SqlException exx)
		{
			System.Diagnostics.Debug.Write(exx.Message);
		}
		return iReturn;
	}

	public int Delete()
	{
		return Delete(this);
	}

	public static int Delete(ProgramGame o )
	{
		int iReturn = -1; //assume the worst

		SqlParameter[] arrParams = new SqlParameter[1];

		arrParams[0]  = new SqlParameter("@PGID", GRA.SRP.Core.Utilities.GlobalUtilities.DBSafeValue(o.PGID, o.PGID.GetTypeCode()));

		try
		{
            var fileName = (HttpContext.Current.Server.MapPath("~/Images/Games/Board/") + "\\" + o.PGID.ToString() + ".png");
            File.Delete(fileName);
            fileName = (HttpContext.Current.Server.MapPath("~/Images/Games/Board/") + "\\bonus_" + o.PGID.ToString() + ".png");
            File.Delete(fileName);
            fileName = (HttpContext.Current.Server.MapPath("~/Images/Games/Board/") + "\\stamp_" + o.PGID.ToString() + ".png");
            File.Delete(fileName);

			iReturn = SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_ProgramGame_Delete", arrParams);
		}
		catch(SqlException exx)
		{
			System.Diagnostics.Debug.Write(exx.Message);
		}
        

		return iReturn;
	}

		#endregion

	}//end class

}//end namespace

