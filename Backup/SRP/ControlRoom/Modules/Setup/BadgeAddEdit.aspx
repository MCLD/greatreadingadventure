<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" CodeBehind="BadgeAddEdit.aspx.cs" 
    Inherits="STG.SRP.ControlRoom.Modules.Setup.BadgeAddEdit" 
    
%>
<%@ Register src="~/Controls/FileUploadCtl.ascx" tagname="FileUploadCtl" tagprefix="uc1" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<meta Http-Equiv="Cache-Control" Content="no-cache">
<meta Http-Equiv="Pragma" Content="no-cache">
<meta Http-Equiv="Expires" Content="0">
<meta Http-Equiv="Pragma-directive: no-cache">
<meta Http-Equiv="Cache-directive: no-cache">

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
        Width="100%">
        <Fields>

        <asp:BoundField DataField="BID" HeaderText="Badge Id: " SortExpression="BID" ReadOnly="True" InsertVisible="False" Visible="false">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>

        <asp:TemplateField  ShowHeader="False" >
            <InsertItemTemplate>
                <table width="100%">
                    <tr>
                        <td nowrap>
                            <b>Badge Control Room Name: </b>
                        </td>
                        <td width=100%>
                            <asp:TextBox ID="AdminName" runat="server" Text='<%# Bind("AdminName") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                                ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="Badge Control Room Name is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                
                    <tr>
                        <td >
                            <b>Badge Patron Name: </b>
                        </td>
                        <td>
                            <asp:TextBox ID="UserName" runat="server" Text='<%# Bind("UserName") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvUserName" runat="server" 
                                ControlToValidate="UserName" Display="Dynamic" ErrorMessage="Badge Patron Name is required" 
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </td>
                    </tr>    
                    
                    <tr>
                        <td colspan="2" >
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
                                    <td nowrap>
                                        <b>Control Room Name: </b>
                                    </td>
                                    <td width=50%>
                                        <asp:TextBox ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>' Width="300px" ></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                                            ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="Badge Control Room Name is required" 
                                            SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                                    </td>  
                                    <td rowspan="4" align="center">

                                        <uc1:FileUploadCtl ID="FileUploadCtl" runat="server" 
                                                FileName='<%# Eval("BID") %>'
                                                ImgWidth="200" 
                                                CreateSmallThumbnail="True" 
                                                CreateMediumThumbnail="False"
                                                SmallThumbnailWidth="64" 
                                                MediumThumbnailWidth="128"
                                                Folder="~/Images/Badges/"
                                                Extension="png"
                                            />
                                    </td>
                                    </td>                                 
                                </tr>
                
                                <tr>
                                    <td >
                                        <b>Patron Web Name: </b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="UserName" runat="server" Text='<%# Eval("UserName") %>' Width="300px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server" 
                                            ControlToValidate="UserName" Display="Dynamic" ErrorMessage="Badge Patron Name is required" 
                                            SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                                    </td>
                                </tr>  
                            
                                <tr>
                                    <td nowrap valign="top">
                                        <b>Awards Physical Prize? </b>
                                    </td>
                                    <td width=50%>
                                        <asp:Checkbox ID="IncludesPhysicalPrizeFlag" runat="server" checked='<%# (bool)Eval("IncludesPhysicalPrizeFlag") %>'></asp:Checkbox>
                                        <br />
                                        &nbsp;&nbsp;&nbsp;&nbsp;<b>Physical Prize Name:</b><br />
                                        &nbsp;&nbsp;&nbsp;<asp:TextBox ID="PhysicalPrizeName" runat="server" Text='<%# Eval("PhysicalPrizeName") %>' Width="285px"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <td  colspan="2">
                                   
                                   <br /><br /><br /><br />
                                        
                                </tr>    
                                
                                <tr>
                                    <td colspan="3" >
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
                                                Height="250px" UIColor="#D3D3D3" 
                                                Visible="True" 
                                                Width="100%"
                                                Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                                / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                                / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                                / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                                "
                                                AutoGrowOnStartup="True" 
                                                Text='<%# Eval("CustomEarnedMessage") %>'
                                                ></CKEditor:CKEditorControl>
                                    </td>
                                </tr>               
                
                            </table>

                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Alert Notification" 
                            ID="TabPanel2"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>
                                <table width="100%">
                                <tr>
                                    <td nowrap>
                                        <b>Send Notification: </b>
                                    </td>
                                    <td width=100%>
                                        <asp:Checkbox ID="GenNotificationFlag" runat="server" checked='<%# (bool)Eval("GenNotificationFlag") %>'></asp:Checkbox>
                                    </td>
                                </tr>
                
                                <tr>
                                    <td >
                                        <b>Notification Subject: </b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="NotificationSubject" runat="server" Text='<%# Eval("NotificationSubject") %>' Width="70%"></asp:TextBox>
                                    </td>
                                </tr>  
                            
                
                                
                                <tr>
                                    <td colspan="2" >
                                        <b>Notification Message :</b>
                                        <br />
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
                                                Height="350px" UIColor="#D3D3D3" 
                                                Visible="True" 
                                                Width="100%"
                                                Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                                / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                                / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                                / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                                "
                                                AutoGrowOnStartup="True" 
                                                Text='<%# Eval("NotificationBody") %>'
                                                ></CKEditor:CKEditorControl>

                                    </td>
                                </tr>               
                
                            </table>    
                            
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>       


                        <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Program Reward Code Assignment" 
                            ID="TabPanel3"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>
                                <table width="100%">
                                <tr>
                                    <td nowrap>
                                        <b>Assign Reward Code: </b>
                                    </td>
                                    <td width=100%>
                                        <asp:Checkbox ID="AssignProgramPrizeCode" runat="server" checked='<%# (bool)Eval("AssignProgramPrizeCode") %>'></asp:Checkbox>
                                    </td>
                                </tr>
                
                                <tr>
                                    <td >
                                        <b>Notification Subject: </b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="PCNotificationSubject" runat="server" Text='<%# Eval("PCNotificationSubject") %>' Width="70%"></asp:TextBox>
                                    </td>
                                </tr>  
                            
                
                                
                                <tr>
                                    <td colspan="2" >
                                        <b>Notification Message Content:</b>
                                        <br />
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
                                                Height="350px" UIColor="#D3D3D3" 
                                                Visible="True" 
                                                Width="100%"
                                                Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                                                / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                                                / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                                                / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                                                "
                                                AutoGrowOnStartup="True" 
                                                Text='<%# Eval("PCNotificationBody") %>'
                                                ></CKEditor:CKEditorControl>

                                    </td>
                                </tr>               
                
                            </table>    
                            
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>      


                        <ajaxToolkit:TabPanel runat="server" 
                            HeaderText="Extended Attributes" 
                            ID="TabPanel4"
                            Enabled="true"
                            ScrollBars="Auto" 
                        >
                            <ContentTemplate>

                            <table width="100%">
                                <tr>
                                    <td nowrap width="50%" align="center">
                                        <b>Badge Category</b>
                                    </td>
                                    <td nowrap width="50%" align="center">
                                        <b>Age Group</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap width="50%" valign="top" height="250px">      
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
                                    <td nowrap width="50%" valign="top" height="250px">
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
                                </tr>

                
                                <tr>
                                    <td nowrap width="50%" align="center">
                                        <b>Branch Library</b>
                                    </td>
                                    <td nowrap width="50%" align="center">
                                        <b>Location</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap width="50%" valign="top" height="250px">      
                                        <div style="height: 250px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd; ">
                                                <asp:GridView ID="gvBranch" ShowHeader=false  runat="server" DataSourceID="odsDDBranch" AutoGenerateColumns="false" Width="100%">
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
                                    <td nowrap width="50%" valign="top" height="250px">
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

                            </table>

                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>

            </EditItemTemplate>
            <ItemTemplate>
                <uc1:FileUploadCtl ID="FileUploadCtl" runat="server" />
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: "  Visible="false"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: "   Visible="false"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: "   Visible="false"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: "   Visible="false"
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
        SelectMethod="FetchObject" TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    
    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetBadgeBranches" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

    <asp:ObjectDataSource ID="odsDDBadgeCat" runat="server" 
        SelectMethod="GetBadgeCategories" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

    <asp:ObjectDataSource ID="odsDDBadgeAge" runat="server" 
        SelectMethod="GetBadgeAgeGroups" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 

    <asp:ObjectDataSource ID="odsDDBadgeLoc" runat="server" 
        SelectMethod="GetBadgeLocations" 
        TypeName="STG.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BID" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource> 
</asp:Content>
