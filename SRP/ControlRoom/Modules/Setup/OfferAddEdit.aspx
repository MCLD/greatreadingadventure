<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="OfferAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.OfferAddEdit" 
    
%>
<%@ Register TagPrefix="uc1" TagName="FileUploadCtl_1" Src="~/Controls/FileUploadCtl.ascx" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


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

        <asp:BoundField DataField="OID" HeaderText="OID: " SortExpression="OID" ReadOnly="True" InsertVisible="False" Visible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            <ItemStyle Width="100%" />
        </asp:BoundField>


        <asp:TemplateField HeaderText="Enabled: " SortExpression="isEnabled" HeaderStyle-Width="100px" >
		    <EditItemTemplate>
                <asp:CheckBox ID="isEnabled" runat="server" Checked='<%# (bool)Eval("isEnabled") %>' ReadOnly="False"></asp:CheckBox>
		    </EditItemTemplate>
            <HeaderStyle VerticalAlign="Bottom" ></HeaderStyle>
            <InsertItemTemplate>
                <asp:CheckBox ID="isEnabled" runat="server" Checked='false' ReadOnly="False"></asp:CheckBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="isEnabled" runat="server" Text='<%# Eval("isEnabled") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Admin Name: " SortExpression="AdminName" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>' ReadOnly="False" width="75%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                    ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color=red> * Required </font></asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>' width="75%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                    ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color=red> * Required </font></asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Title: " SortExpression="Title" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="Title" runat="server" Text='<%# Eval("Title") %>' ReadOnly="False" width="75%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                    ControlToValidate="Title" Display="Dynamic" ErrorMessage="Title is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="Title" runat="server" Text='<%# Eval("Title") %>' width="75%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                    ControlToValidate="Title" Display="Dynamic" ErrorMessage="Title is required" 
                    SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Title" runat="server" Text='<%# Eval("Title") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Redirect Externally: " SortExpression="ExternalRedirectFlag" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:CheckBox ID="ExternalRedirectFlag" runat="server" Checked='<%# (bool)Eval("ExternalRedirectFlag") %>' ReadOnly="False"></asp:CheckBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:CheckBox ID="ExternalRedirectFlag" runat="server" Checked='false' ReadOnly="False"></asp:CheckBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="ExternalRedirectFlag" runat="server" Checked='<%# (bool)Eval("ExternalRedirectFlag") %>' ReadOnly="False"></asp:CheckBox>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="RedirectURL: " SortExpression="RedirectURL" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="RedirectURL" runat="server" Text='<%# Eval("RedirectURL") %>' ReadOnly="False" width="75%"></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="RedirectURL" runat="server" Text='<%# Eval("RedirectURL") %>' width="75%"></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="RedirectURL" runat="server" Text='<%# Eval("RedirectURL") %>' width="75%"></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="MaxImpressions: " SortExpression="MaxImpressions" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="MaxImpressions" runat="server" Text='<%# ((int) Eval("MaxImpressions") ==0 ? "" : Eval("MaxImpressions")) %>'></asp:TextBox>
                
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="MaxImpressions" runat="server" Text='0'></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="MaxImpressions" runat="server" Text='<%# ((int) Eval("MaxImpressions") ==0 ? "" : Eval("MaxImpressions")) %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="TotalImpressions: " SortExpression="TotalImpressions" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <%# FormatHelper.ToInt((int)Eval("TotalImpressions"))%>
            </EditItemTemplate>
            <InsertItemTemplate>
                0
            </InsertItemTemplate>
            <ItemTemplate>
                <%# FormatHelper.ToInt((int)Eval("TotalImpressions"))%>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="SerialPrefix: " SortExpression="SerialPrefix" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="SerialPrefix" runat="server" Text='<%# Eval("SerialPrefix") %>' ReadOnly="False"></asp:TextBox>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="SerialPrefix" runat="server" Text='<%# Eval("SerialPrefix") %>'></asp:TextBox>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="SerialPrefix" runat="server" Text='<%# Eval("SerialPrefix") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        
        <asp:TemplateField InsertVisible="False" ShowHeader="False"  HeaderText="Offer Image: " >
		    <EditItemTemplate>
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
            </EditItemTemplate>
            <InsertItemTemplate>
            </InsertItemTemplate>
            <ItemTemplate>
                <uc1:FileUploadCtl_1 ID="FileUploadCtl" runat="server" />
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="ZipCode: " SortExpression="ZipCode" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="ZipCode" runat="server" Text='<%# Eval("ZipCode") %>' ReadOnly="False"></asp:TextBox>

            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="ZipCode" runat="server" Text='<%# Eval("ZipCode") %>'></asp:TextBox>

            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="ZipCode" runat="server" Text='<%# Eval("ZipCode") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="AgeStart: " SortExpression="AgeStart" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:TextBox ID="AgeStart" runat="server" Text='<%# ((int) Eval("AgeStart") ==0 ? "" : Eval("AgeStart")) %>' ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                 <asp:RegularExpressionValidator id="revAgeStart"
                    ControlToValidate="AgeStart"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color=red>Age Start must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Age Start must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
             <asp:RangeValidator ID="rvAgeStart"
                    ControlToValidate="AgeStart"
                    MinimumValue="1"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color=red>Age Start must be from 1 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Age Start must be from 1 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />     
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AgeStart" runat="server" Text='' ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                 <asp:RegularExpressionValidator id="revAgeStart"
                    ControlToValidate="AgeStart"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color=red>Age Start must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Age Start must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
             <asp:RangeValidator ID="rvAgeStart"
                    ControlToValidate="AgeStart"
                    MinimumValue="1"
                    MaximumValue="99"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color=red>Age Start must be from 1 to 99!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color=red> * Age Start must be from 1 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" /> 
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AgeStart" runat="server" Text='<%# ((int) Eval("AgeStart") ==0 ? "" : Eval("AgeStart")) %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="AgeEnd: " SortExpression="AgeEnd" HeaderStyle-Width="100px">
		    <EditItemTemplate>

                <asp:TextBox ID="AgeEnd" runat="server" Text='<%# ((int) Eval("AgeEnd") ==0 ? "" : Eval("AgeEnd")) %>' ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
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
                    Font-Bold="True" Font-Italic="True" 
                    runat="server" 
                    Text="<font color=red> * Age End must be from 1 to 99! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                      
                                   
                                   
                                                    
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="AgeEnd" runat="server" Text='' ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
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
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="AgeEnd" runat="server" Text='<%# ((int) Eval("AgeEnd") ==0 ? "" : Eval("AgeEnd")) %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="ProgramId: " SortExpression="ProgramId" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                
                <asp:DropDownList ID="ProgramId" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="ProgramIdLbl" runat="server" Text='<%# Eval("ProgramId") %>' Visible="false" ></asp:Label>

            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="ProgramId" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
                <asp:Label ID="ProgramId" runat="server" Text='<%# Eval("ProgramId") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="BranchId: " SortExpression="BranchId" HeaderStyle-Width="100px">
		    <EditItemTemplate>
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                 >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="BranchIdLbl" runat="server" Text='<%# Eval("BranchId") %>' Visible="false" ></asp:Label>
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" AppendDataBoundItems="True">
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
            </InsertItemTemplate>
            <ItemTemplate>
            </ItemTemplate>
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>


            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: "  Visible="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: "  Visible="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: "  Visible="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: "  Visible="False"
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
        TypeName="GRA.SRP.DAL.Offer">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="OID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsProg" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>

</asp:Content>

