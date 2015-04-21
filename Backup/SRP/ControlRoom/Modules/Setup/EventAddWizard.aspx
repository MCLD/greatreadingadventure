<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="EventAddWizard.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.EventAddWizard" 
    
%>
<%@ Import Namespace="STG.SRP.DAL" %>
    <%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
    <%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<%@ Register src="../../Controls/EvtCustFldCtl.ascx" tagname="EvtCustFldCtl" tagprefix="uc1" %>

<%@ Register TagPrefix="uc1" TagName="FileUploadCtl" Src="~/Controls/FileUploadCtl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

<asp:Panel ID="pnlEvent" runat="server" BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Event Details" ScrollBars="Auto"
    DefaultButton="btnContinue" CssClass="OrangePanel" width="100%" height="585px">
    <table width="100%" height="550px">
        <tr>
            <td nowrap> <b> Event Title: </b> </td>
            <td colspan="7" >
                    <asp:TextBox ID="EventTitle" runat="server" Width="99%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server" 
                    ControlToValidate="EventTitle" Display="Dynamic" ErrorMessage="<font color=red>Event Title is required</font>" 
                    SetFocusOnError="True" Font-Bold="True"><font color=red>* Required</font></asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td  nowrap> <b> Start Date: </b> </td>
            <td>
                <asp:TextBox ID="EventDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceEventDate" runat="server" TargetControlID="EventDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meEventDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="EventDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>    
                <asp:RequiredFieldValidator ID="rfvEventDate" runat="server" 
                    ControlToValidate="EventDate" Display="Dynamic" ErrorMessage="<font color=red>Event Date is required</font>" 
                    SetFocusOnError="True" Font-Bold="True"><font color=red>* Required</font></asp:RequiredFieldValidator>
            </td>
            <td  nowrap> <b> Start Time: </b> </td>
            <td>
                <asp:TextBox ID="EventTime" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEventTime" runat="server" 
                    ControlToValidate="EventTime" Display="Dynamic" ErrorMessage="<font color=red>Start Time is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color=red>* Required</font></asp:RequiredFieldValidator>                        
            </td>

            <td  nowrap> <b> End Date: </b> </td>
            <td>
                <asp:TextBox ID="EndDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="EndDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>    
            </td>
            <td  nowrap> <b> End Time: </b> </td>
            <td>
                <asp:TextBox ID="EndTime" runat="server"></asp:TextBox>                         
            </td>
        </tr>
        <tr>
            <td  nowrap> <b> Short Description: </b> </td>
            <td colspan="7">
                <asp:TextBox ID="ShortDescription" runat="server" Width="99%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td  nowrap valign="top"> <b> Event Description: </b> </td>
            <td colspan="7" style="height: 350px; ">
                <CKEditor:CKEditorControl ID="HTML" 
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
            </td>
        </tr>   
                    


        <tr>
            <td  nowrap> <b> Branch/Library: </b> </td>
            <td >
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" Width="254"
                    AppendDataBoundItems="True"
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </td>
            
        </tr>

        <tr>
            <td nowrap> <b> Custom 1: </b> </td>
            <td> <uc1:EvtCustFldCtl ID="Custom1" runat="server" Value='' FieldNumber="1"  /> </td>

            <td  nowrap> <b> Custom 2: </b> </td>
            <td> <uc1:EvtCustFldCtl ID="Custom2" runat="server" Value='' FieldNumber="2"  /> </td>

            <td nowrap> <b> Custom 3: </b> </td>
            <td> <uc1:EvtCustFldCtl ID="Custom3" runat="server" Value='' FieldNumber="3"  /> </td>

        </tr>

        <tr>
            <td colspan="6" height="100%" valign="bottom">
                <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnBack_Click" />
                &nbsp;&nbsp;

                &nbsp;&nbsp;
                &nbsp;&nbsp;
                &nbsp;&nbsp;

                &nbsp;

                <asp:ImageButton ID="btnContinue" runat="server" 
                        CausesValidation="True"                         
                        ImageUrl="~/ControlRoom/Images/Next_sm.png" 
                        Height="25"
                        Text="Continue"  Tooltip="Continue"
                        AlternateText="Continue" onclick="btnContinue_Click" />
            </td>
        </tr>
                                 
    </table>
                    
</asp:Panel>

<asp:Panel ID="pnlReward" runat="server" Visible="False" BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Event Attendance Rewards" ScrollBars="Auto"
    DefaultButton="btnContinue2" CssClass="OrangePanel"  width="100%" height="585px">
    <table width="100%" height="550px">    
        <tr>
            <td  nowrap> <b> Secret Code: </b> </td>
            <td >
                <asp:TextBox ID="SecretCode" runat="server" Width="250" AutoPostBack="true" 
                    ontextchanged="SecretCode_TextChanged"></asp:TextBox> 
            </td>
            <td colspan="4"> 
            <asp:Label ID="lblDups" runat="server" Text=""></asp:Label>
            This is the code patrons will log as proof of attendance.</td>
        </tr>
        <tr>
            <td  nowrap> <b> Number Points: </b> </td>
            <td  valign="top">
                <asp:TextBox ID="NumberPoints" runat="server" 
                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>                
            </td>
            <td colspan="4">
            <asp:RegularExpressionValidator id="revNumberPoints"
                    ControlToValidate="NumberPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color=red>Number Points must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Number Points must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumberPoints"
                    ControlToValidate="NumberPoints"
                    MinimumValue="0"
                    MaximumValue="9999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color=red>Number Points must be from 0 to 9999!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Number Points must be from 0 to 9999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />
                This is the points the patrons will receive when they log/report the secret code.
            </td>
        </tr>
        <tr>
            <td colspan="6"><hr /></td>
        </tr>
        <tr>
            <td  nowrap valign="top"><br /><b>  Badge Awarded: </b> </td>
            <td  valign="top" align="left" width="250px;">
                <asp:RadioButtonList ID="rblBadge" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="rblBadge_SelectedIndexChanged" Font-Size="14px">
                    <asp:ListItem Selected="false" Value="0" Text="  &nbsp; No Badge Awarded"></asp:ListItem>
                    <asp:ListItem Selected="true" Value="1" Text="  &nbsp; Choose An Existing Badge"></asp:ListItem>
                    <asp:ListItem Selected="false" Value="2" Text="  &nbsp; Create A New Badge"></asp:ListItem>
                </asp:RadioButtonList>
                *Note: If you choose to create a new badge, you need to have the badge image ready for upload on the next step.
            </td>
            <td colspan="4" valign="top" width="900px">
            <img src="/controlroom/images/spacer.gif" height="23px" /><br />
                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID"  Width="500" Visible="true"
                    AppendDataBoundItems="True"
                    >
                    <asp:ListItem Value="0" Text="[Select a Badge]"></asp:ListItem>
                </asp:DropDownList>            
            </td>
        </tr>
        <tr>
            <td colspan="6">
                
                <asp:Panel ID="pnlBadge" runat="server" GroupingText=" New Badge Info" ScrollBars="Auto" Visible="False">

                <table width="100%">
                    <tr>
                        <td nowrap valign="top">
                            <b>Control Room Name: </b>
                        </td>
                        <td width="40%">
                            <asp:TextBox ID="AdminName" runat="server" Text='' Width="300px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" Enabled="False"
                                ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="<font color='red'>Badge Control Room Name is required</font>" 
                                SetFocusOnError="True" Font-Bold="True"><font color='red'>* Required</asp:RequiredFieldValidator>
                        </td>
                        <td nowrap valign="top">
                            <b>Patron Web Name: </b>
                        </td>
                        <td width="40%">
                            <asp:TextBox ID="UserName" runat="server" Text='' Width="300px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvUserName" runat="server" Enabled="False"
                                ControlToValidate="UserName" Display="Dynamic" ErrorMessage="<font color='red'>Badge Patron Name is required</font>" 
                                SetFocusOnError="True" Font-Bold="True"><font color='red'>* Required</font></asp:RequiredFieldValidator>
                        </td>
                    </tr>    
                    <tr>
                        <td colspan="4"><hr /></td>
                    </tr>
                    <tr>
                        <td colspan="4" height="160px">
                            <b>Message to Patron when badge is earned:</b>
                            <br />
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
                                    UIColor="#D3D3D3" 
                                    Visible="True" 
                                    Width="98%" Height="110px"
                                    Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                    / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                    / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                    / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                    "
                                    AutoGrowOnStartup="False" 
     
                                    ></CKEditor:CKEditorControl>

                        </td>
                    </tr>                                  
                
                </table>


                </asp:Panel>
            
            
            </td>
        </tr>
        <tr>
            <td colspan="6" height="100%" valign="bottom">
                <asp:ImageButton ID="btnCancel2" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnCancel2_Click"  />
                &nbsp;&nbsp;
                
                <asp:ImageButton ID="btnPrevious2" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/Previous_sm.png" 
                        Height="25"
                        Text="Previous"  Tooltip="Previous"
                        AlternateText="Previous" onclick="btnPrevious2_Click"  />

                        &nbsp;&nbsp;
    
                    <asp:ImageButton ID="btnContinue2" runat="server" 
                        CausesValidation="True"                         
                        ImageUrl="~/ControlRoom/Images/Next_sm.png" 
                        Height="25"
                        Text="Continue"  Tooltip="Continue"
                        AlternateText="Continue" onclick="btnContinue2_Click"  />
                
            </td>
        </tr>                                 
    </table>
</asp:Panel>

<asp:Panel ID="pnlBadgeMore" runat="server" BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Event Details" ScrollBars="Auto"
    DefaultButton="btnContinue" CssClass="OrangePanel" width="100%" height="585px" Visible="false">
    <table width="100%" height="550px">
        <tr>
            <td nowrap> <b> Awards Physical Prize?: </b> </td>
            <td colspan="1" >
                    <asp:Checkbox ID="IncludesPhysicalPrizeFlag" runat="server" checked='False'></asp:Checkbox>
            </td>
            <td nowrap> <b> Awarded Physical Prize Name: </b> </td>
            <td colspan="1" >
                    <asp:TextBox ID="PhysicalPrizeName" runat="server" Text='' Width="285px"></asp:TextBox>
            </td>        
        </tr>
        <tr>
            <td nowrap> <b> Send Notification?: </b> </td>
            <td colspan="3" >
                    <asp:Checkbox ID="GenNotificationFlag" runat="server" checked='False'></asp:Checkbox>
            </td>
        </tr>
        <tr>
            <td nowrap> <b>  Notification Subject: </b> </td>
            <td colspan="3" >
                    <asp:TextBox ID="NotificationSubject" runat="server" Text='' Width="70%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td  nowrap colspan="4" height="125px"> 
                    <b> Notification Message Text: </b> <br />
                <CKEditor:CKEditorControl ID="NotificationBody" 
                                                BasePath="/ckeditor/" 
                                                runat="server" 
                                                Skin="office2003" 
         
                                                BodyId="wrapper" 
                                                ContentsCss="/css/EditorStyles.css" 
                                                DisableNativeSpellChecker="False" 
                                                DisableNativeTableHandles="False" 
                                                DocType="&lt;!DOCTYPE html&gt;" 
                                                ForcePasteAsPlainText="True" 
                                                Height="80px" UIColor="#D3D3D3" 
                                                Visible="True" 
                                                Width="100%"
                                                Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                                / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                                / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                                / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                                "
                                                AutoGrowOnStartup="True" 
                                                Text=''
                                                ></CKEditor:CKEditorControl>            
            </td>
        </tr>
        
        <tr>
            <td nowrap> <b> Assign Pgm. Reward Code?: </b> </td>
            <td colspan="3" >
                    <asp:Checkbox ID="AssignProgramPrizeCode" runat="server" Checked="false"></asp:Checkbox>
            </td>
        </tr>
        <tr>
            <td nowrap> <b>  Notification Subject: </b> </td>
            <td colspan="3" >
                    <asp:TextBox ID="PCNotificationSubject" runat="server" Text='' Width="70%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td  nowrap colspan="4" height="125px"> 
                    <b> Notification Message Text: </b> <br />
                <CKEditor:CKEditorControl ID="PCNotificationBody" 
                                                BasePath="/ckeditor/" 
                                                runat="server" 
                                                Skin="office2003" 
         
                                                BodyId="wrapper" 
                                                ContentsCss="/css/EditorStyles.css" 
                                                DisableNativeSpellChecker="False" 
                                                DisableNativeTableHandles="False" 
                                                DocType="&lt;!DOCTYPE html&gt;" 
                                                ForcePasteAsPlainText="True" 
                                                Height="80px" UIColor="#D3D3D3" 
                                                Visible="True" 
                                                Width="100%"
                                                Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                                / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                                / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                                / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                                "
                                                AutoGrowOnStartup="True" 
                                                Text=''
                                                ></CKEditor:CKEditorControl>            
            </td>
        </tr>

        <tr>
            <td colspan="6" height="100%" valign="bottom">
                <asp:ImageButton ID="btnCancel3" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnCancel3_Click"   />
                &nbsp;&nbsp;
                
                <asp:ImageButton ID="btnPrevious3" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/Previous_sm.png" 
                        Height="25"
                        Text="Previous"  Tooltip="Previous"
                        AlternateText="Previous" onclick="btnPrevious3_Click"  />

                        &nbsp;&nbsp;
    
                    <asp:ImageButton ID="btnContinue3" runat="server" 
                        CausesValidation="True"                         
                        ImageUrl="~/ControlRoom/Images/Next_sm.png" 
                        Height="25"
                        Text="Continue"  Tooltip="Continue"
                        AlternateText="Continue" onclick="btnContinue3_Click"   />
                
            </td>
        </tr> 
    </table>




                    
</asp:Panel>

<asp:Panel ID="pnlLast" runat="server" BorderColor="#3399FF" BorderStyle="Solid" BorderWidth="0px" GroupingText=" Event Details" ScrollBars="Auto"
    DefaultButton="btnContinue" CssClass="OrangePanel" width="100%" height="585px" Visible="false">
    <table width="100%" height="550px">

                                <tr>
                                    <td nowrap width="25%" align="center">
                                        <b>Badge Category</b>
                                    </td>
                                    <td nowrap width="25%" align="center">
                                        <b>Age Group</b>
                                    </td>
                                    <td nowrap width="25%" align="center">
                                        <b>Branch Library</b>
                                    </td>
                                    <td nowrap width="25%" align="center">
                                        <b>Location</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap width="25%" valign="top" height="250px">      
                                        <div style="height: 250px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd; ">
                                                <asp:GridView ID="gvCat" ShowHeader=false  runat="server" DataSourceID="odsDDBadgeCat" AutoGenerateColumns="false" Width="100%">
                                                <Columns>
                                                    <asp:TemplateField ShowHeader="false">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("Checked")).ToString()=="1"?true:false) %>' runat="server" />   
                                                        <%# Eval("Name")%>
                                                        <asp:Label ID="CID" runat="server" Text='<%# Eval("CID") %>' Visible="False"></asp:Label>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>       
                                        </div>             
                                               
                                    </td>
                                    <td nowrap width="25%" valign="top" height="250px">
                                        <div style="height: 250px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd; ">
                                                <asp:GridView ID="gvAge" ShowHeader=false  runat="server" DataSourceID="odsDDBadgeAge" AutoGenerateColumns="false" Width="100%">
                                                <Columns>
                                                    <asp:TemplateField ShowHeader="false">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("Checked")).ToString()=="1"?true:false) %>' runat="server" />   
                                                        <%# Eval("Name")%>
                                                        <asp:Label ID="CID" runat="server" Text='<%# Eval("CID") %>' Visible="False"></asp:Label>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>       
                                        </div>             

                                    </td>
                                
                                    <td nowrap width="25%" valign="top" height="250px">      
                                        <div style="height: 250px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd; ">
                                                <asp:GridView ID="gvBranch" ShowHeader=false  runat="server" DataSourceID="odsDDBranch2" AutoGenerateColumns="false" Width="100%">
                                                <Columns>
                                                    <asp:TemplateField ShowHeader="false">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("Checked")).ToString()=="1"?true:false) %>' runat="server" />   
                                                        <%# Eval("Name")%>
                                                        <asp:Label ID="CID" runat="server" Text='<%# Eval("CID") %>' Visible="False"></asp:Label>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>       
                                        </div>             
                                               
                                    </td>
                                    <td nowrap width="25%" valign="top" height="250px">
                                        <div style="height: 250px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd; ">
                                                <asp:GridView ID="gvLoc" ShowHeader=false  runat="server" DataSourceID="odsDDBadgeLoc" AutoGenerateColumns="false" Width="100%">
                                                <Columns>
                                                    <asp:TemplateField ShowHeader="false">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("Checked")).ToString()=="1"?true:false) %>' runat="server" />   
                                                        <%# Eval("Name")%>
                                                        <asp:Label ID="CID" runat="server" Text='<%# Eval("CID") %>' Visible="False"></asp:Label>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>       
                                        </div>  
                                    </td>
                                </tr>

        <tr>
            <td colspan = "4"> <b> Badge Image: <br />
                <uc1:FileUploadCtl ID="FileUploadCtl" runat="server" 
                    CreateMediumThumbnail="False" CreateSmallThumbnail="True" Extension="png" 
                    FileName="0" Folder="~/Images/Badges/" ImgWidth="200" 
                    MediumThumbnailWidth="128" SmallThumbnailWidth="64" />
                                                                      
            </td>        
        </tr>

        <tr>
            <td colspan="4" height="100%" valign="bottom">
                <asp:ImageButton ID="btnCancel4" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnCancel4_Click"  />
                &nbsp;&nbsp;
                
                <asp:ImageButton ID="btnPrevious4" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/Previous_sm.png" 
                        Height="25"
                        Text="Previous"  Tooltip="Previous"
                        AlternateText="Previous" onclick="btnPrevious4_Click"   />

                        &nbsp;&nbsp;
    
                    <asp:ImageButton ID="btnContinue4" runat="server" 
                        CausesValidation="True"                         
                        ImageUrl="~/ControlRoom/Images/Next_sm.png" 
                        Height="25"
                        Text="Continue"  Tooltip="Continue"
                        AlternateText="Continue" onclick="btnContinue4_Click"  />
                
            </td>
        </tr>  

    </table>
</asp:Panel>



    <asp:Label ID="lblPK" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="lblBID" runat="server" Text="0" Visible="False"></asp:Label>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="FetchObject" 
        TypeName="STG.SRP.DAL.Event">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="EID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>



    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsBadge" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Badge">
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch2" runat="server" 
        SelectMethod="GetBadgeBranches" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblBID" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

    <asp:ObjectDataSource ID="odsDDBadgeCat" runat="server" 
        SelectMethod="GetBadgeCategories" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblBID" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

    <asp:ObjectDataSource ID="odsDDBadgeAge" runat="server" 
        SelectMethod="GetBadgeAgeGroups" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblBID" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

    <asp:ObjectDataSource ID="odsDDBadgeLoc" runat="server" 
        SelectMethod="GetBadgeLocations" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblBID" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 
  
</asp:Content>

