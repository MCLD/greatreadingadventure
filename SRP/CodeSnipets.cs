using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.SRP
{
public class CodeSnipets
{
    /*
    <%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>

    -----------------------------------------------------------------------------------------------------------------------
    -- Addressing tabs
    -----------------------------------------------------------------------------------------------------------------------
    obj.IncludesPhysicalPrizeFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1").FindControl("CheckBox")).Checked;

    -----------------------------------------------------------------------------------------------------------------------
    -- AJAX
    -----------------------------------------------------------------------------------------------------------------------
    <%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
         
    -----------------------------------------------------------------------------------------------------------------------
    -- Calendar Field
    -----------------------------------------------------------------------------------------------------------------------
    <asp:TextBox ID="txtPPRR" runat="server" Width="75px"                         
        Text='<%# (Eval("PrevProgRevDate").ToString()=="" ? "" : DateTime.Parse(Eval("PrevProgRevDate").ToString()).ToShortDateString() ) %>'
    ></asp:TextBox>
    <ajaxToolkit:CalendarExtender ID="cePPRR" runat="server" TargetControlID="txtPPRR">
    </ajaxToolkit:CalendarExtender>
    <ajaxToolkit:MaskedEditExtender ID="mePPRR" runat="server" 
        UserDateFormat="MonthDayYear" TargetControlID="txtPPRR" MaskType="Date" Mask="99/99/9999">
    </ajaxToolkit:MaskedEditExtender>     
 
 
    obj.EventDate = FormatHelper.SafeToDateTime(((TextBox)((DetailsView)sender).FindControl("EventDate")).Text);
 
    
    -----------------------------------------------------------------------------------------------------------------------
    -- Integer Field
    -----------------------------------------------------------------------------------------------------------------------
 
    <asp:TextBox ID="AgeEnd" runat="server" 
        Text='<%# ((int) Eval("AgeEnd") ==0 ? "" : Eval("AgeEnd")) %>' 
        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
        <asp:RegularExpressionValidator id="revAgeEnd"
        ControlToValidate="AgeEnd"
        ValidationExpression="\d+"
        Display="Dynamic"
        EnableClientScript="true"
        ErrorMessage="<font color=red>Age End must be numeric.</font>"
        runat="server"
        Font-Bold="True" Font-Italic="True" 
        Text="<font color=red> * Age End must be numeric. </font>" 
        EnableTheming="True" 
        SetFocusOnError="True" />      
    <asp:RangeValidator ID="rvAgeEnd"
        ControlToValidate="AgeEnd"
        MinimumValue="1"
        MaximumValue="99"
        Display="Dynamic"
        Type="Integer"
        EnableClientScript="true"
        ErrorMessage="<font color=red>Age End must be from 1 to 99!</font>"
        runat="server" 
        Font-Bold="True" Font-Italic="True" 
        Text="<font color=red> * Age End must be from 1 to 99! </font>" 
        EnableTheming="True" 
        SetFocusOnError="True" /> 
 
 
   -----------------------------------------------------------------------------------------------------------------------
    -- HTML Field
    ----------------------------------------------------------------------------------------------------------------------- 
    <%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

    <CKEditor:CKEditorControl ID="CustomEarnedMessage" 
            BasePath="/ckeditor/" 
            runat="server" 
            Skin="office2003" 
         
            BodyId="wrapper" 
            ContentsCss="/css/EditorStyles.css" 
            DisableNativeSpellChecker="False" 
            DisableNativeTableHandles="False" 
            DocType="&lt;!DOCTYPE html&gt;" 
            ForcePasteAsPlainText="True" 
            Height="250px" UIColor="#D3D3D3" 
            Visible="True" 
            Width="98%"
            Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
            / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
            / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
            / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
            "
            AutoGrowOnStartup="True" 
     
            ></CKEditor:CKEditorControl>

    obj.CustomEarnedMessage = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("CustomEarnedMessage")).Text;



    -----------------------------------------------------------------------------------------------------------------------
    File Upload
    -----------------------------------------------------------------------------------------------------------------------
    <%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>

    <uc1:FileUploadCtl_1 ID="FileUploadCtl" runat="server" 
            FileName='<%# Eval("OID") %>'
            ImgWidth="512" 
            CreateSmallThumbnail="True" 
            CreateMediumThumbnail="True"
            SmallThumbnailWidth="64" 
            MediumThumbnailWidth="128"
            Folder="~/Images/Offers/"
            Extension="png"
        />
    protected void dv_DataBound(object sender, EventArgs e)
    {
        if (dv.CurrentMode == DetailsViewMode.Edit)
        {
            var control = (STG.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
            if (control!=null) control.ProcessRender();
        }
    }

    <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
        <HeaderTemplate>
                    
        </HeaderTemplate>                
        <ItemTemplate>
            <asp:Image ID="Image1" runat="server" ImageUrl='<%# String.Format("~/Images/Avatars/sm_{0}.png?{1}", Eval("AID").ToString(), DateTime.Now.ToString()) %>' />
        </ItemTemplate>
        <ItemStyle VerticalAlign="Top" Wrap="False" Width="300px" HorizontalAlign="Center"></ItemStyle>
    </asp:TemplateField>


    -----------------------------------------------------------------------------------------------------------------------
    -- Drop Down Edit (and Add - w/o the label)
    -----------------------------------------------------------------------------------------------------------------------
    <asp:DropDownList ID="ProgramId" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
        AppendDataBoundItems="True"
        >
        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="ProgramIdLbl" runat="server" Text='<%# Eval("ProgramId") %>' Visible="False"></asp:Label>    



    <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
        AppendDataBoundItems="True"
        >
        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
    </asp:DropDownList>
    <asp:Label ID="BranchIdLbl" runat="server" Text='<%# Eval("BranchId") %>' Visible="False"></asp:Label>
    
    obj.ProgramId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("ProgramId")).SelectedValue);
    obj.BranchId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BranchId")).SelectedValue);

    protected void dv_DataBound(object sender, EventArgs e)
    {
        if (dv.CurrentMode == DetailsViewMode.Edit)
        {
            var ctl = (DropDownList)dv.FindControl("BranchId"); //this.FindControlRecursive(this, "BranchId");
            var lbl = (Label)dv.FindControl("BranchIdLbl"); 
            var i = ctl.Items.FindByValue(lbl.Text);
            if (i != null) ctl.SelectedValue = lbl.Text;

            ctl = (DropDownList)dv.FindControl("ProgramId"); 
            lbl = (Label)dv.FindControl("ProgramIdLbl");
            i = ctl.Items.FindByValue(lbl.Text);
            if (i != null) ctl.SelectedValue = lbl.Text;
        }
    }
 
    

   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsProg" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Programs">
    </asp:ObjectDataSource>

    -----------------------------------------------------------------------------------------------------------------------

    -----------------------------------------------------------------------------------------------------------------------
    -- Search 
    -----------------------------------------------------------------------------------------------------------------------
<table>
<thead>
    <th colspan="7">
    </th>
</thead>
<tr>
<td><b>Start Date:</b> </td>
<td>
                <asp:TextBox ID="StartDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceStartDate" runat="server" TargetControlID="StartDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meStartDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="StartDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>  
</td>
<td><b>End Date:</b> </td>
<td>
                <asp:TextBox ID="EndDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceEndDate" runat="server" TargetControlID="EndDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meEndDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>  
</td>

<td><b>Branch:</b> </td>
<td>
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True" Width="200px"
                 >
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
</td>
<td>
    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click" />
        &nbsp;
    <asp:Button ID="btnClear" runat="server" Text="Clear/All" onclick="btnClear_Click" 
         />

</td>
</tr>
</table>  







    <asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAdminSearch" 
        TypeName="STG.SRP.DAL.Event">
        <SelectParameters>
            <asp:ControlParameter ControlID="StartDate" DefaultValue="" Name="startDate" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="EndDate" Name="endDate" PropertyName="Text" 
                Type="String" />
            <asp:ControlParameter ControlID="BranchId" DefaultValue="0" Name="branchID" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>




Alter PROCEDURE [dbo].[app_Event_GetAdminSearch]
  @startDate datetime
, @endDate datetime
, @branchID int
 AS
   Select * 
	, (select Code from dbo.Code where CID = BranchID ) as Branch
   from [Event]
   where 
		(BranchID = @branchID or @branchID = 0)
		AND (EventDate >= @startDate or @startDate is null)
		AND (EventDate <= @endDate or @endDate is null)
   order by EventDate desc
go







        public static DataSet GetAdminSearch(string startDate, string endDate, int branchID)
        {
            SqlParameter[] arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@startDate", GlobalUtilities.DBSafeDate(startDate));
            arrParams[1] = new SqlParameter("@endDate", GlobalUtilities.DBSafeDate(endDate)); // (string.IsNullOrEmpty(endDate) ? (object)DBNull.Value : DateTime.Parse(endDate)));
            arrParams[2] = new SqlParameter("@branchID", branchID);

            return SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_Event_GetAdminSearch", arrParams);
        }





            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                if (WasFiltered())
                {
                    GetFilterSessionValues();
                }
            }

        public void SetFilterSessionValues()
        {
            Session["EL_Start"] = StartDate.Text;
            Session["EL_End"] = EndDate.Text;
            Session["EL_Branch"] = BranchId.SelectedValue;
            Session["EL_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["EL_Start"] != null) StartDate.Text = Session["EL_Start"].ToString();
            if (Session["EL_End"] != null) EndDate.Text = Session["EL_End"].ToString();
            if (Session["EL_Branch"] != null) try { BranchId.SelectedValue = Session["EL_Branch"].ToString(); }
                catch (Exception) { }
        }

        public bool WasFiltered()
        {
            return (Session["EL_Filtered"] != null);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            SetFilterSessionValues();
            odsData.DataBind();
            gv.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            StartDate.Text = EndDate.Text = "";
            BranchId.SelectedValue = "0";
            SetFilterSessionValues();
            odsData.DataBind();
            gv.DataBind();
        }
 





    -----------------------------------------------------------------------------------------------------------------------
    -- Move Up, Down and fix order sprocs 
    -----------------------------------------------------------------------------------------------------------------------
Alter Procedure [dbo].[app_Programs_Reorder]
AS
	
	UPDATE Programs 
	SET POrder = rowNumber 
	FROM Programs
		INNER JOIN 
		(SELECT PID, POrder, row_number() OVER (ORDER BY POrder Asc) as rowNumber
			FROM Programs) drRowNumbers ON drRowNumbers.PID = Programs.PID
	
	
GO


Alter PROCEDURE [dbo].[app_Programs_MoveUp]
@PID int 
AS
	exec [dbo].[app_Programs_Reorder]
	Declare @CurrentRecordLocation int, @PreviousRecordID int
	Select @CurrentRecordLocation = POrder from Programs where PID = @PID
	if @CurrentRecordLocation > 1
	begin
		Select @PreviousRecordID = PID from Programs where POrder = (@CurrentRecordLocation - 1)
		
		update Programs set POrder = @CurrentRecordLocation - 1
		where PID = @PID
	
		update Programs set POrder = @CurrentRecordLocation 
		where PID = @PreviousRecordID
	end
GO

Alter PROCEDURE [dbo].[app_Programs_MoveDn]
@PID int 
AS
	exec [dbo].[app_Programs_Reorder]
	
	Declare @CurrentRecordLocation int, @NextRecordID int
	Select @CurrentRecordLocation = POrder from Programs where PID = @PID
	if @CurrentRecordLocation < (Select MAX(POrder) from Programs)
	begin
		Select @NextRecordID = PID from Programs where POrder = (@CurrentRecordLocation + 1)
		
		update Programs set POrder = @CurrentRecordLocation + 1
		where PID = @PID
	
		update Programs set POrder = @CurrentRecordLocation 
		where PID = @NextRecordID
	end

GO
    -----------------------------------------------------------------------------------------------------------------------

                    <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="DOB" 
                    ErrorMessage="Invalid Date of Birth format" Type="Date" 
                    Operator="DataTypeCheck" Display="Dynamic" Text="* Invalid format" ForeColor="Red" ></asp:CompareValidator>


     string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
     * 
     * 
     <a href="https://www.facebook.com/sharer/sharer.php?u=http://www.mydomain.com/mypage.htm" target="_blank">
                      Share on Facebook
                    </a>
     * 
             
    foreach (EMyEnum val in Enum.GetValues(typeof(EMyEnum)))
    {
       Console.WriteLine(val);
    }

      
-- generate x rows/numbers
DECLARE @start INT = 1;
DECLARE @end INT = 10000;

WITH numbers AS (
    SELECT @start AS Number
    UNION ALL
    SELECT number + 1 
    FROM  numbers
    WHERE number < @end
)
SELECT *, NEWID() as Code
FROM numbers
OPTION (MAXRECURSION 0);
      
      foreach (RepeaterItem item in rptItems.Items)
{
    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
    {
        var checkBox = (CheckBox)item.FindControl("ckbActive");

        //Do something with your checkbox...
        checkBox.Checked = true;
    }
}
      
      
      
     */

}
}