<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="EventAddEdit.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.EventAddEdit" 
    
%>
<%@ Import Namespace="STG.SRP.DAL" %>
    <%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
    <%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<%@ Register src="../../Controls/EvtCustFldCtl.ascx" tagname="EvtCustFldCtl" tagprefix="uc1" %>

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

        <asp:BoundField DataField="EID" HeaderText="Event ID: " SortExpression="EID" ReadOnly="True" InsertVisible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>


        <asp:TemplateField HeaderText="Event Title: " SortExpression="EventTitle" >
		    <EditItemTemplate>
                <asp:TextBox ID="EventTitle" runat="server" Text='<%# Eval("EventTitle") %>' ReadOnly="False" Width="75%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server" 
                    ControlToValidate="EventTitle" Display="Dynamic" ErrorMessage="EventTitle is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </EditItemTemplate>

            <InsertItemTemplate>
                <asp:TextBox ID="EventTitle" runat="server" Text='<%# Eval("EventTitle") %>' Width="75%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEventTitle" runat="server" 
                    ControlToValidate="EventTitle" Display="Dynamic" ErrorMessage="EventTitle is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="EventTitle" runat="server" Text='<%# Eval("EventTitle") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Date: " SortExpression="EventDate" >
		    <EditItemTemplate>
                <asp:TextBox ID="EventDate" runat="server" Width="75px"                         
                    Text='<%# (Eval("EventDate").ToString()=="" ? "" : DateTime.Parse(Eval("EventDate").ToString()).ToShortDateString() ) %>'></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceEventDate" runat="server" TargetControlID="EventDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meEventDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="EventDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>    
                <asp:RequiredFieldValidator ID="rfvEventDate" runat="server" 
                    ControlToValidate="EventDate" Display="Dynamic" ErrorMessage="EventDate is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="EventDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceEventDate" runat="server" TargetControlID="EventDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meEventDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="EventDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>    
                <asp:RequiredFieldValidator ID="rfvEventDate" runat="server" 
                    ControlToValidate="EventDate" Display="Dynamic" ErrorMessage="EventDate is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="EventDate" runat="server" Text='<%# Eval("EventDate") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Time: " SortExpression="EventTime" >
		    <EditItemTemplate>
                <asp:TextBox ID="EventTime" runat="server" Text='<%# Eval("EventTime") %>' ReadOnly="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEventTime" runat="server" 
                    ControlToValidate="EventTime" Display="Dynamic" ErrorMessage="EventTime is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="EventTime" runat="server" Text='<%# Eval("EventTime") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEventTime" runat="server" 
                    ControlToValidate="EventTime" Display="Dynamic" ErrorMessage="EventTime is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="EventTime" runat="server" Text='<%# Eval("EventTime") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Description: " SortExpression="HTML" >
		    <EditItemTemplate>
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
                        Height="150px" UIColor="#D3D3D3" 
                        Visible="True" 
                        Width="98%"
                        Toolbar="Source|-|Preview|-|Cut|Copy|Paste|PasteText|PasteFromWord|-|Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat| 
                        / |Bold|Italic|Underline|Strike|-|Subscript|Superscript|-|NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv|-|JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock| 
                        / |Link|Unlink|Anchor|-|Image|Flash|Table|HorizontalRule|SpecialChar|PageBreak|Iframe|
                        / |Styles|Format|Font|FontSize|-|TextColor|BGColor|-|ShowBlocks|Maximize|
                        "
                        AutoGrowOnStartup="True" 
                        Text='<%# Eval("HTML") %>'
                        ></CKEditor:CKEditorControl>
            </EditItemTemplate>
            <InsertItemTemplate>
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
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="HTML" runat="server" Text='<%# Eval("HTML") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Secret Code: " SortExpression="SecretCode" >
		    <EditItemTemplate>
                <asp:TextBox ID="SecretCode" runat="server" Text='<%# Eval("SecretCode") %>' ReadOnly="False"  Width="50%"></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="SecretCode" runat="server" Text='<%# Eval("SecretCode") %>'   Width="50%"></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="SecretCode" runat="server" Text='<%# Eval("SecretCode") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Number Points: " SortExpression="NumberPoints" >
		    <EditItemTemplate>
                <asp:TextBox ID="NumberPoints" runat="server" Text='<%# ((int) Eval("NumberPoints") ==0 ? "" : Eval("NumberPoints")) %>' 
                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
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
                    ErrorMessage="<font color=red>Number Points must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Number Points must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
        
            </EditItemTemplate>
            <InsertItemTemplate>
                 <asp:TextBox ID="NumberPoints" runat="server" 
                    ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
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
                    ErrorMessage="<font color=red>Number Points must be from 0 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Number Points must be from 0 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />             </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="NumberPoints" runat="server" Text='<%# Eval("NumberPoints") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Badge Awarded: " SortExpression="BadgeID" HeaderStyle-Wrap="False">
		    <EditItemTemplate>
                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="BadgeIDLbl" runat="server" Text='<%# Eval("BadgeID") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
               <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsBadge" DataTextField="AdminName" DataValueField="BID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BadgeID" runat="server" Text='<%# Eval("BadgeID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Branch: " SortExpression="BranchID" >
		    <EditItemTemplate>
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="BranchIdLbl" runat="server" Text='<%# Eval("BranchId") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="BranchID" runat="server" Text='<%# Eval("BranchID") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Custom1: " SortExpression="Custom1" >
		    <EditItemTemplate>
                <uc1:EvtCustFldCtl ID="Custom1" runat="server" Value='<%# Eval("Custom1") %>' FieldNumber="1"  />
            </EditItemTemplate>
            <InsertItemTemplate>
                <uc1:EvtCustFldCtl ID="Custom1" runat="server" Value='' FieldNumber="1"  />
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Custom1" runat="server" Text='<%# Eval("Custom1") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Custom2: "  SortExpression="Custom2" >
		    <EditItemTemplate>
                <uc1:EvtCustFldCtl ID="Custom2" runat="server" Value='<%# Eval("Custom2") %>' FieldNumber="2"  />
            </EditItemTemplate>
            <InsertItemTemplate>
                <uc1:EvtCustFldCtl ID="Custom2" runat="server" Value='' FieldNumber="2"  />
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Custom2" runat="server" Text='<%# Eval("Custom2") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
               
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Custom3: " SortExpression="Custom3" >
		    <EditItemTemplate>
                <uc1:EvtCustFldCtl ID="Custom3" runat="server" Value='<%# Eval("Custom3") %>' FieldNumber="3"  />
            </EditItemTemplate>
            <InsertItemTemplate>
                <uc1:EvtCustFldCtl ID="Custom3" runat="server" Value='' FieldNumber="3"  />
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Custom3" runat="server" Text='<%# Eval("Custom3") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " 
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " 
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " 
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " 
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
    
</asp:Content>

