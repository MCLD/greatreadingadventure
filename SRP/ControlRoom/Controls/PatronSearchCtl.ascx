<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronSearchCtl.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.PatronSearchCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<table width="100%" style="border: double 3px #A3C0E8;">
    <tr>
        <th>First Name</th>
        <th>Last Name</th>
        <th>Username/Login ID</th>
    </tr>
    <tr>
        <td>
            <asp:TextBox ID="txtFirstName" runat="server" Width="95%" CssClass="form-control"></asp:TextBox></td>
        <td>
            <asp:TextBox ID="txtLastName" runat="server" Width="95%" CssClass="form-control"></asp:TextBox></td>
        <td>
            <asp:TextBox ID="txtUsername" runat="server" Width="95%" CssClass="form-control"></asp:TextBox></td>
    </tr>

    <tr>
        <th colspan="2">Email Address</th>
        <th>Library District</th>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox ID="txtEmail" runat="server" Width="97%" CssClass="form-control"></asp:TextBox></td>
        <td>
            <asp:TextBox ID="txtDOB" runat="server" Width="75px" CssClass="form-control"
                Text='' Visible="false"></asp:TextBox>
            <!--<ajaxToolkit:CalendarExtender ID="cetxtDOB" runat="server" TargetControlID="txtDOB"></ajaxToolkit:CalendarExtender>
            <ajaxToolkit:MaskedEditExtender ID="metxtDOB" runat="server"
                UserDateFormat="MonthDayYear" TargetControlID="txtDOB" MaskType="Date" Mask="99/99/9999"></ajaxToolkit:MaskedEditExtender>-->
            <asp:DropDownList ID="LibraryDistrictId"
                runat="server"
                CssClass="form-control"
                DataSourceID="LibraryDistrictData"
                DataTextField="Code"
                DataValueField="CID"
                AppendDataBoundItems="True"
                AutoPostBack="true">
                <asp:ListItem Value="" Text="[Select a Library District]"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th colspan="2">Program</th>
        <th>Branch</th>
    </tr>
    <tr>
        <td colspan="2">
            <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms"
                DataTextField="TabName" DataValueField="PID" CssClass="form-control"
                AppendDataBoundItems="True" Width="97%" OnDataBound="ProgID_DataBound">
                <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <asp:DropDownList ID="Gender" runat="server" CssClass="form-control" Visible="false"
                AppendDataBoundItems="True" Width="95%">
                <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
                <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                <asp:ListItem Value="O" Text="Other"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="LibraryId"
                runat="server"
                CssClass="form-control"
                DataSourceID="LibraryBranchData"
                DataTextField="Code"
                DataValueField="CID"
                AppendDataBoundItems="True"
                AutoPostBack="true">
                <asp:ListItem Value="" Text="[Select a Library Branch]"></asp:ListItem>
            </asp:DropDownList>

        </td>
    </tr>

</table>

<asp:ObjectDataSource ID="odsDDPrograms" runat="server"
    SelectMethod="GetAll"
    TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>

<asp:ObjectDataSource ID="LibraryDistrictData" runat="server"
    SelectMethod="GetFilteredDistrictDDValues"
    TypeName="GRA.SRP.DAL.LibraryCrosswalk">
    <SelectParameters>
        <asp:Parameter Name="city" DefaultValue="" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:HiddenField runat="server" ID="SelectedDistrictId" />

<asp:ObjectDataSource ID="LibraryBranchData" runat="server"
    SelectMethod="GetFilteredBranchDDValues"
    TypeName="GRA.SRP.DAL.LibraryCrosswalk">
    <SelectParameters>
        <asp:ControlParameter ControlID="SelectedDistrictId" DefaultValue="0" Name="districtId"
            PropertyName="Value" Type="Int32" />
        <asp:Parameter Name="city" DefaultValue="" />
    </SelectParameters>
</asp:ObjectDataSource>
