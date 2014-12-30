﻿<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="ProgramsAddEdit.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Programs.ProgramsAddEdit" 
    
%>
<%@ Import Namespace="STG.SRP.DAL" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        onitemcommand="DvItemCommand" ondatabinding="dv_DataBinding" 
        ondatabound="dv_DataBound"
        Width="100%"
        >
        <Fields>

        <asp:BoundField DataField="PID" HeaderText="PID: " SortExpression="PID" ReadOnly="True" InsertVisible="False" Visible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>

        <asp:TemplateField  ShowHeader="False" >
            <InsertItemTemplate>
                <table width="100%">
                    <tr>
                        <td align="right" nowrap>
                            <b>Control Room Name: </b>
                        </td>
                        <td width=100% colspan="3">
                            
                            <asp:TextBox ID="AdminName" runat="server" Text='<%# Bind("AdminName") %>' Width="80%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                                ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="Control Room Name is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                
                    <tr>
                        <td align="right">
                            <b>Program Title: </b>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="Title" runat="server" Text='<%# Bind("Title") %>' Width="80%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                                ControlToValidate="Title" Display="Dynamic" ErrorMessage="Title is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>   
                    
                    <tr>
                        <td align="right">
                            <b>Tab Name: </b>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TabName" runat="server" Text='<%# Bind("TabName") %>' Width="80%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTabName" runat="server" 
                                ControlToValidate="TabName" Display="Dynamic" ErrorMessage="Tab Name is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>    
                    
                    <tr>
                        <td align="right">
                            <b>Start Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="StartDate" runat="server" Text='' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceStartDate" runat="server" TargetControlID="StartDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meStartDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="StartDate" MaskType="Date" Mask="99/99/9999" ClearMaskOnLostFocus="True">
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" 
                                ControlToValidate="StartDate" Display="Dynamic" ErrorMessage="StartDate is required" 
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <td align="right">
                            <b>End Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="EndDate" runat="server" Text='' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceEndDate" runat="server" TargetControlID="EndDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meEndDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999" ClearMaskOnLostFocus="True">
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" 
                                ControlToValidate="EndDate" Display="Dynamic" ErrorMessage="EndDate is required" 
                                SetFocusOnError="False" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                    </tr>                                                   
                    
                    <tr>
                        <td align="right">
                            <b>Logging Start Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="LoggingStart" runat="server" Text='' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceLoggingStart" runat="server" TargetControlID="LoggingStart">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meLoggingStart" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="LoggingStart" MaskType="Date" Mask="99/99/9999" 
                                ClearMaskOnLostFocus="True" ClearTextOnInvalid="true" >
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvLoggingStart" runat="server" 
                                ControlToValidate="LoggingStart" Display="Dynamic" ErrorMessage="LoggingStart is required" 
                                SetFocusOnError="False" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <td align="right">
                            <b>Logging End Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="LoggingEnd" runat="server" Text='' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceLoggingEnd" runat="server" TargetControlID="LoggingEnd">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meLoggingEnd" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="LoggingEnd" MaskType="Date" Mask="99/99/9999" ClearMaskOnLostFocus="True">
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvLoggingEnd" runat="server" 
                                ControlToValidate="LoggingEnd" Display="Dynamic" ErrorMessage="LoggingEnd is required" 
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                    </tr>   
                    
                    <tr>
                        <td align="right">
                            <b>Is Active: </b>
                        </td>
                        <td >
                            <asp:CheckBox ID="IsActive" runat="server" ReadOnly="False"></asp:CheckBox>
                        </td>
                        <td align="right">
                            <b>Is Hidden: </b>
                        </td>
                        <td >
                            <asp:CheckBox ID="IsHidden" runat="server" ReadOnly="False"></asp:CheckBox>
                        </td>
                    </tr> 
                       
                    <tr>
                        <td align="left" colspan="4">
                            <b>Program Introduction text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML1" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="150px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    ></CKEditor:CKEditorControl>
                        </td>
                    </tr>

                    <tr>
                        <td align="left" colspan="4">
                        <br />
                            <b>Program Sponsor text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML2" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="150px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    ></CKEditor:CKEditorControl>
                        </td>
                    </tr>
                                                                                                                      
                </table>
            </InsertItemTemplate>  
            <EditItemTemplate>
                <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
                    Height="600px"
                    Width="100%"
                    AutoPostBack="false"
                    TabStripPlacement="Top"
                    CssClass="ajax__tab_xp"
                    ScrollBars="None"
                    UseVerticalStripPlacement="false"
                    VerticalStripWidth="120px"
                    >
                        <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Basic Information" 
                            ID="TabPanel1"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>
          <table width="100%">
                    <tr>
                        <td align="right" nowrap>
                            <b>Control Room Name: </b>
                        </td>
                        <td width="700px" colspan="3">
                            <asp:TextBox ID="PID" runat="server" Text='<%# Bind("PID") %>' Visible="False"></asp:TextBox>
                            <asp:TextBox ID="AdminName" runat="server" Text='<%# Bind("AdminName") %>' Width="90%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                                ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="Control Room Name is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                        <td rowspan="9" width="300px" align="left" valign=top>
                        
                                         &nbsp;<b>Banner </b>
                                      <uc1:FileUploadCtl_1 ID="FileUploadCtl" runat="server" 
                                            FileName='<%# Eval("PID") %>'
                                            ImgWidth="1170" 
                                            ImgHeight="200"
                                            CreateSmallThumbnail="False" 
                                            CreateMediumThumbnail="False"
                                            SmallThumbnailWidth="64" 
                                            MediumThumbnailWidth="128"
                                            Folder="~/Images/Banners/"
                                            Extension="png"
                                        />
                     
                        </td>
                    </tr>
                
                    <tr>
                        <td align="right">
                            <b>Program Title: </b>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="Title" runat="server" Text='<%# Bind("Title") %>' Width="90%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                                ControlToValidate="Title" Display="Dynamic" ErrorMessage="Title is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>   
                    
                    <tr>
                        <td align="right">
                            <b>Tab Name: </b>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TabName" runat="server" Text='<%# Bind("TabName") %>' Width="90%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTabName" runat="server" 
                                ControlToValidate="TabName" Display="Dynamic" ErrorMessage="Tab Name is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>    
                    
                    <tr>
                        <td align="right">
                            <b>DIRECT LINK: </b>
                        </td>
                        <td colspan="3">
                            <a href='<%# "/Default.aspx?PID=" + Eval("PID").ToString() %>' target=_blank><%# Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/Default.aspx?PID=" + Eval("PID").ToString() %></a>
                        </td>
                    </tr> 
                    
                    <tr>
                        <td align="right">
                            <b>Start Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="StartDate" runat="server" Text='<%# (Eval("StartDate").ToString()=="" ? "" : DateTime.Parse(Eval("StartDate").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceStartDate" runat="server" TargetControlID="StartDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meStartDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="StartDate" MaskType="Date" Mask="99/99/9999" ClearMaskOnLostFocus="True">
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" 
                                ControlToValidate="StartDate" Display="Dynamic" ErrorMessage="StartDate is required" 
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <td align="right">
                            <b>End Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="EndDate" runat="server" Text='<%# (Eval("EndDate").ToString()=="" ? "" : DateTime.Parse(Eval("EndDate").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceEndDate" runat="server" TargetControlID="EndDate">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meEndDate" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999" ClearMaskOnLostFocus="True">
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" 
                                ControlToValidate="EndDate" Display="Dynamic" ErrorMessage="EndDate is required" 
                                SetFocusOnError="False" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                    </tr>                                                   
                    
                    <tr>
                        <td align="right">
                            <b>Logging Start Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="LoggingStart" runat="server" Text='<%# (Eval("LoggingStart").ToString()=="" ? "" : DateTime.Parse(Eval("LoggingStart").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceLoggingStart" runat="server" TargetControlID="LoggingStart">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meLoggingStart" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="LoggingStart" MaskType="Date" Mask="99/99/9999" 
                                ClearMaskOnLostFocus="True" ClearTextOnInvalid="true" >
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvLoggingStart" runat="server" 
                                ControlToValidate="LoggingStart" Display="Dynamic" ErrorMessage="LoggingStart is required" 
                                SetFocusOnError="False" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                        <td align="right">
                            <b>Logging End Date: </b>
                        </td>
                        <td >
                            <asp:TextBox ID="LoggingEnd" runat="server" Text='<%# (Eval("LoggingEnd").ToString()=="" ? "" : DateTime.Parse(Eval("LoggingEnd").ToString()).ToShortDateString() ) %>' ReadOnly="False" Width="80px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="ceLoggingEnd" runat="server" TargetControlID="LoggingEnd">
                            </ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:MaskedEditExtender ID="meLoggingEnd" runat="server" 
                                UserDateFormat="MonthDayYear" TargetControlID="LoggingEnd" MaskType="Date" Mask="99/99/9999" ClearMaskOnLostFocus="True">
                            </ajaxToolkit:MaskedEditExtender>    
                            <asp:RequiredFieldValidator ID="rfvLoggingEnd" runat="server" 
                                ControlToValidate="LoggingEnd" Display="Dynamic" ErrorMessage="LoggingEnd is required" 
                                SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                        </td>
                    </tr>   
                    
                    <tr>
                        <td align="right">
                            <b>Is Active: </b>
                        </td>
                        <td >
                            <asp:CheckBox ID="IsActive" runat="server" Checked='<%# (bool)Eval("IsActive") %>' ReadOnly="False"></asp:CheckBox>
                        </td>
                        <td align="right">
                            <b>Is Hidden: </b>
                        </td>
                        <td >
                            <asp:CheckBox ID="IsHidden" runat="server" Checked='<%# (bool)Eval("IsHidden") %>' ReadOnly="False"></asp:CheckBox>
                        </td>
                    </tr> 
    
                    <tr>
                        <td align="right">
                            <b>Max Age: </b>
                        </td>
                        <td >
                <asp:TextBox ID="MaxAge" runat="server" Text='<%# ((int) Eval("MaxAge") ==0 ? "" : Eval("MaxAge")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="revMaxAge"
                    ControlToValidate="MaxAge"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>MaxAge must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * MaxAge must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvMaxAge"
                    ControlToValidate="MaxAge"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>MaxAge must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * MaxAge must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                             
                        </td>
                        <td align="right">
                            <b>Max Grade: </b>
                        </td>
                        <td >
                <asp:TextBox ID="MaxGrade" runat="server" Text='<%# ((int) Eval("MaxGrade") ==0 ? "" : Eval("MaxGrade")) %>' 
                     ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                <asp:RegularExpressionValidator id="revMaxGrade"
                    ControlToValidate="MaxGrade"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>MaxGrade must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * MaxGrade must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvMaxGrade"
                    ControlToValidate="MaxGrade"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>MaxGrade must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * MaxGrade must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                         </td>
                    </tr>                        
                   
                      <tr>
                        <td align="right">
                            <b>Consent Flag: </b>
                        </td>
                        <td >
                            <asp:CheckBox ID="ParentalConsentFlag" runat="server" Checked='<%# (bool)Eval("ParentalConsentFlag") %>' ReadOnly="False"></asp:CheckBox>
                        </td>
                        <td align="right">                      
                        </td>
                        <td >
                            
                        </td>
                    </tr>  
                      <tr>
                        <td align="right">
                            <b>Consent Text: </b>
                        </td>
                        <td colspan=4>
                <CKEditor:CKEditorControl ID="ParentalConsentText" 
                        BasePath="/ckeditor/" 
                        runat="server" 
                        Skin="office2003" 
                        BodyId="wrapper" 
                        ContentsCss="/css/EditorStyles.css"
                        DisableNativeSpellChecker="False" 
                        DisableNativeTableHandles="False" 
                        DocType="&lt;!DOCTYPE html&gt;" 
                        ForcePasteAsPlainText="True" 
                        Height="150px" UIColor="#D3D3D3" 
                        Visible="True" 
                        Width="98%"
                        Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                        / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                        / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                        / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                        "
                        AutoGrowOnStartup="True" 
                        Text='<%# Eval("ParentalConsentText") %>' 
                        ></CKEditor:CKEditorControl>                            

                            
                        </td>
                    </tr>
                      <tr>
                        <td align="right">
                            <b>User Can Enter Review: </b>
                        </td>
                        <td >
                            <asp:CheckBox ID="PatronReviewFlag" runat="server" Checked='<%# (bool)Eval("PatronReviewFlag") %>' ReadOnly="False"></asp:CheckBox>
                        </td>
                        <td align="right">
                            
                        </td>
                        <td >
                            
                        </td>
                    </tr>  
                      <tr>
                        <td align="right">
                            <b>Logout URL: </b>
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="LogoutURL" runat="server" Text='<%# Eval("LogoutURL") %>' ReadOnly="False" Width="80%"></asp:TextBox>
                        
                        </td>
                        <td >
                            
                        </td>
                    </tr>                                                                                                                                                            
                </table>
                                            
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                        <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Description & Game" 
                            ID="TabPanel2"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>                                                
   <table width="100%">
   
                <tr>
                    <tr>
                        <td align="right" width="100px">
                            <b>Badge: </b>
                        </td>
                        <td >
                            <asp:DropDownList ID="RegistrationBadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID" 
                                AppendDataBoundItems="True" Width="600px"
                                >
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="RegistrationBadgeIDLbl" runat="server" Text='<%# Eval("RegistrationBadgeID") %>' Visible="False"></asp:Label>                            
                        </td>
                        <td></td>
                        <td></td>
                    </tr> 
                    <tr>
                        <td align="right">
                            <b>Game: </b>
                        </td>
                        <td >
                            <asp:DropDownList ID="ProgramGameID" runat="server" DataSourceID="odsDDGames" DataTextField="GameName" DataValueField="PGID" 
                                AppendDataBoundItems="True" Width="600px"
                                >
                                <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="ProgramGameIDLbl" runat="server" Text='<%# Eval("ProgramGameID") %>' Visible="False"></asp:Label>                               
                        </td>
                        <td align="right">
                            <b>Game Completion Points: </b>
                        </td>
                        <td>
                        
                            <asp:TextBox ID="CompletionPoints" runat="server" Text='<%# ((int) Eval("CompletionPoints") ==0 ? "" : Eval("CompletionPoints")) %>' 
                                 ReadOnly="False" Width="80px" CssClass="align-right"></asp:TextBox>

                            <asp:RegularExpressionValidator id="RegularExpressionValidator1"
                                ControlToValidate="CompletionPoints"
                                ValidationExpression="\d+"
                                Display="Dynamic"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Completion Points must be numeric.</font>"
                                runat="server"
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * Completion Points must be numeric. </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />      
                            <asp:RangeValidator ID="RangeValidator1"
                                ControlToValidate="CompletionPoints"
                                MinimumValue="0"
                                MaximumValue="99999"
                                Display="Dynamic"
                                Type="Integer"
                                EnableClientScript="true"
                                ErrorMessage="<font color='red'>Completion Points must be from 0 to 99,999!</font>"
                                runat="server" 
                                Font-Bold="True" Font-Italic="True" 
                                Text="<font color='red'> * 0 to 99,999! </font>" 
                                EnableTheming="True" 
                                SetFocusOnError="True" />  

                        </td>
                    </tr>
                    
                                         
                        <td align="left" colspan="4">
                            <b>Program Introduction text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML1" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="150px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    Text='<%# Eval("HTML1") %>' 
                                    ></CKEditor:CKEditorControl>
                        </td>
                    </tr>

                    <tr>
                        <td align="left" colspan="4">
                        <br />
                            <b>Program Sponsor text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML2" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="150px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    Text='<%# Eval("HTML2") %>' 
                                    ></CKEditor:CKEditorControl>
                        </td>
                    </tr>

   </table>                     
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>       








                       <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Side Text" 
                            ID="TabPanel3"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>                                                
   <table width="100%">
   
    <tr>
                                        
                        <td align="left" valign="top">
                            <b>Left Column Text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML3" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="350px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    Text='<%# Eval("HTML3") %>' 
                                    ></CKEditor:CKEditorControl>
                        </td>

                        <td align="left" valign="top" >
                        
                            <b>Right Column Text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML4" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="350px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    Text='<%# Eval("HTML4") %>' 
                                    ></CKEditor:CKEditorControl>
                        </td>
                    </tr>

   </table>                     
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>   












                       <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Footer & Not Logging Text" 
                            ID="TabPanel4"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>                                                
   <table width="100%">
   
    <tr>
                                        
                        <td align="left" valign="top">
                            <b>Footer Text: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML5" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="150px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    Text='<%# Eval("HTML5") %>' 
                                    ></CKEditor:CKEditorControl>
                        </td>
                        </tr><tr>
                        <td align="left" valign="top" >
                        
                            <b>Notification When Logging Is Not Active: </b><br />
                        
                            <CKEditor:CKEditorControl ID="HTML6" 
                                    BasePath="/ckeditor/" 
                                    runat="server" 
                                    Skin="office2003" 
                                    BodyId="wrapper" 
                                    ContentsCss="/css/EditorStyles.css"
                                    DisableNativeSpellChecker="False" 
                                    DisableNativeTableHandles="False" 
                                    DocType="&lt;!DOCTYPE html&gt;" 
                                    ForcePasteAsPlainText="True" 
                                    Height="200px" UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="True" 
                                    Text='<%# Eval("HTML6") %>' 
                                    ></CKEditor:CKEditorControl>
                        </td>
                    </tr>

   </table>                     
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>   




                       <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Activity Point Conversions" 
                            ID="TabPanel5"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>     
                                                                       
                                <table width="">
                                    <thead>
                                        <th colspan="2">
                                            &nbsp;&nbsp;This Much of this Activity&nbsp;&nbsp;
                                        </th>
                                        <th align="center">
                                            EQUALS
                                        </th>
                                        <th colspan="2">
                                            &nbsp;&nbsp;This many Points&nbsp;&nbsp;
                                        </th>
                                    </thead>



                                <asp:Repeater ID="rptr" runat="server" DataSourceID="odsPtConv">
                                    <ItemTemplate>
                                        <tr>
                                            <td align="left" valign="top">  
                                                <asp:TextBox ID="PGCID" runat="server" Text='<%# Eval("PGCID") %>' Visible="False"></asp:TextBox>
                                                                                      
                                                <asp:TextBox ID="ActivityCount" runat="server" Text='<%# ((int) Eval("ActivityCount") ==0 ? "" : Eval("ActivityCount")) %>' 
                                                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                                                <asp:RegularExpressionValidator id="revActivityCount"
                                                    ControlToValidate="ActivityCount"
                                                    ValidationExpression="\d+"
                                                    Display="Dynamic"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Activity Count must be numeric.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True" 
                                                    Text="<font color='red'> * Activity Count must be numeric. </font>" 
                                                    EnableTheming="True" 
                                                    SetFocusOnError="True" />      
                                                <asp:RangeValidator ID="rvActivityCount"
                                                    ControlToValidate="ActivityCount"
                                                    MinimumValue="1"
                                                    MaximumValue="9999"
                                                    Display="Dynamic"
                                                    Type="Integer"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Activity Count must be from 1 to 9999!</font>"
                                                    runat="server" 
                                                    Font-Bold="True" Font-Italic="True" 
                                                    Text="<font color='red'> * Activity Count must be from 1 to 9999! </font>" 
                                                    EnableTheming="True" 
                                                    SetFocusOnError="True" />   
                    
                                    
                                            </td>
                                            <td align="left" valign="top">                                            
                                                <%# ((ActivityType)Eval("ActivityTypeId")).ToString()%>
                                            </td>
                                            <td align="left" valign="top">                                            
                                                EQUALS
                                            </td>
                                            <td align="left" valign="top">                                            
                                                <asp:TextBox ID="PointCount" runat="server" Text='<%# ((int) Eval("PointCount") ==0 ? "" : Eval("PointCount")) %>' 
                                                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>

                                                <asp:RegularExpressionValidator id="revPointCount"
                                                    ControlToValidate="PointCount"
                                                    ValidationExpression="\d+"
                                                    Display="Dynamic"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Point Count must be numeric.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True" 
                                                    Text="<font color='red'> * Point Count must be numeric. </font>" 
                                                    EnableTheming="True" 
                                                    SetFocusOnError="True" />      
                                                <asp:RangeValidator ID="rvPointCount"
                                                    ControlToValidate="PointCount"
                                                    MinimumValue="0"
                                                    MaximumValue="9999"
                                                    Display="Dynamic"
                                                    Type="Integer"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>Point Count must be from 0 to 9999!</font>"
                                                    runat="server" 
                                                    Font-Bold="True" Font-Italic="True" 
                                                    Text="<font color='red'> * Point Count must be from 0 to 9999! </font>" 
                                                    EnableTheming="True" 
                                                    SetFocusOnError="True" />   
                    
                                    
                                            </td>
                                            <td align="left" valign="top">                                            
                                                POINTS
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>


                                </table>                     
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>   



                        <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Program Reward Codes" 
                            ID="TabPanel6"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>     
                                                                       
                                <table width="">
                                    <thead>
                                        <th align="center" colspan="2">
                                            &nbsp;&nbsp;Total Codes&nbsp;&nbsp;
                                        </th>
                                        <th align="center">
                                            Used Codes
                                        </th>
                                        <th align="center" colspan="2">
                                            &nbsp;&nbsp;Remaining Codes&nbsp;&nbsp;
                                        </th>
                                        <th align="center" colspan="2">
                                            &nbsp;&nbsp;Last Used Code&nbsp;&nbsp;
                                        </th>
                                    </thead>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lblTotalCodes" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="lblUsedCodes" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lblRemainingCodes" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td align="center" colspan="2">
                                            <asp:Label ID="lblLastCode" runat="server" Text=""></asp:Label>
                                        </td>                                    
                                    </tr>
                                    <tr><td colspan = 7><hr /></td></tr>
                                    <tr>
                                        <td colspan = 3><b>Add This Many More Codes:</b></td>
                                        <td>
                                            <asp:TextBox ID="txtGen" runat="server" Text="0"></asp:TextBox>

                                                <asp:RegularExpressionValidator id="revtxtGen"
                                                    ControlToValidate="txtGen"
                                                    ValidationExpression="\d+"
                                                    Display="Dynamic"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>The number of codes to generate must be numeric.</font>"
                                                    runat="server"
                                                    Font-Bold="True" Font-Italic="True" 
                                                    Text="<font color='red'> * The number of codes to generate must be numeric. </font>" 
                                                    EnableTheming="True" 
                                                    SetFocusOnError="True" />      
                                                <asp:RangeValidator ID="rvtxtGen"
                                                    ControlToValidate="txtGen"
                                                    MinimumValue="0"
                                                    MaximumValue="9999"
                                                    Display="Dynamic"
                                                    Type="Integer"
                                                    EnableClientScript="true"
                                                    ErrorMessage="<font color='red'>The number of codes to generate must be from 0 to 9999!</font>"
                                                    runat="server" 
                                                    Font-Bold="True" Font-Italic="True" 
                                                    Text="<font color='red'> * The number of codes to generate must be from 0 to 9999! </font>" 
                                                    EnableTheming="True" 
                                                    SetFocusOnError="True" />   
                                        </td>
                                        <td colspan=4>
                                            <asp:Button ID="btnGen" runat="server" Text="Generate Codes" CommandName="gen" />
                                        </td>
                                    </tr>
                                    <tr><td colspan = 7><hr /></td></tr>
                                    <tr><td colspan = 7>
                                    
                                        <asp:Button ID="btnExport" runat="server" Text="Export Codes" CommandName="exp" />

                                    </td></tr>


                                </table>                     
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>   


                </ajaxToolkit:TabContainer>
                
            </EditItemTemplate>
            <ItemTemplate>
          
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

   

 

            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False" Visible="false"
                SortExpression="AddedUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel"    Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                        &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" 
                        CausesValidation="True" 
                        CommandName="Add" 
                        ImageUrl="~/ControlRoom/Images/add.png" 
                        Height="25"
                        Text="Add"   Tooltip="Add"
                        AlternateText="Add" /> 

                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"   Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Text="Refresh Record"    Tooltip="Refresh Record"
                        AlternateText="Refresh Record" /> 
                        &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server" 
                        CausesValidation="True" 
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png" 
                        Height="25"
                        Text="Save"   Tooltip="Save"
                        AlternateText="Save"/>  
                        &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Text="Save and return"   Tooltip="Save and return"
                        AlternateText="Save and return" />    
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="STG.SRP.DAL.Programs">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

	<asp:ObjectDataSource ID="odsPtConv" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.ProgramGamePointConversion">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PGID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>


   <asp:ObjectDataSource ID="odsDDBadges" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Badge">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDGames" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.ProgramGame">
    </asp:ObjectDataSource>

</asp:Content>

