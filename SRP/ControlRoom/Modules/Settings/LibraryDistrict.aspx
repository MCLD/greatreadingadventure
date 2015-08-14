<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="LibraryDistrict.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.LibraryDistrict" 
    
%>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


<table>
    <tr>
        <td><b>Branch</b></td>
        <td><b>District</b></td>
        <td><b>City</b></td>
    </tr>

<asp:Repeater ID="rptrCW" runat="server" onitemdatabound="rptrCW_ItemDataBound">
    <ItemTemplate>
        <tr>
            <td>
                <asp:Label ID="ID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='False' 
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="BranchIDTxt" runat="server" Text='<%# ((int) Eval("BranchID") ==0 ? "" : Eval("BranchID")) %>' Visible="false"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="DistrictID" runat="server" DataSourceID="odsDDDistrict" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True"
                    Enabled='True' 
                    >
                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="DistrictIDTxt" runat="server" Text='<%# ((int) Eval("DistrictID") ==0 ? "" : Eval("DistrictID")) %>' Visible="false"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' Visible="True" Width="200px"></asp:TextBox>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
</table>

    <asp:ImageButton ID="btnBack" runat="server" 
        CausesValidation="false" 
        CommandName="Back" 
        ImageUrl="~/ControlRoom/Images/back.png" 
        Height="25"
        Text="Back/Cancel"   Tooltip="Back/Cancel"
        AlternateText="Back/Cancel" onclick="btnBack_Click" />
        &nbsp;
        &nbsp;
    <asp:ImageButton ID="btnRefresh" runat="server" 
        CausesValidation="false" 
        CommandName="Refresh" 
        ImageUrl="~/ControlRoom/Images/refresh.png" 
        Height="25"
        Text="Refresh Record"    Tooltip="Refresh Record"
        AlternateText="Refresh Record" onclick="btnRefresh_Click" /> 
        &nbsp;
    <asp:ImageButton ID="btnSave" runat="server" 
        CausesValidation="True" 
        CommandName="Save"
        ImageUrl="~/ControlRoom/Images/save.png" 
        Height="25"
        Text="Save"   Tooltip="Save"
        AlternateText="Save" onclick="btnSave_Click"/>  
        &nbsp;
    <asp:ImageButton ID="btnSaveback" runat="server" 
        CausesValidation="True" 
        CommandName="Saveandback" 
        ImageUrl="~/ControlRoom/Images/saveback.png" 
        Height="25"
        Text="Save and return"   Tooltip="Save and return"
        AlternateText="Save and return" onclick="btnSaveback_Click" />    
                

	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.LibraryCrosswalk">
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDDistrict" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>  

   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>
    