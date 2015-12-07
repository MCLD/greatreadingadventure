<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="ProgramsDelete.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Programs.ProgramsDelete" 
    
%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .btn-red
        {
            height: 26px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />
<asp:Label ID="lblProg" runat="server" Text="" ></asp:Label>
<hr />
<table>
    <tr>
        <td>Move the Patrons in this Pogram to the following Program: </td>
        <td>
            <asp:DropDownList ID="PatronProgram" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                AppendDataBoundItems="True"
                >
                <asp:ListItem Value="0" Text="[No Other Program]"></asp:ListItem>
            </asp:DropDownList>
    </td>
    </tr>
    <tr>
        <td>On Physical Prize Drawing Templates replace this Program to the following Program: </td>
        <td>
            <asp:DropDownList ID="PrizeProgram" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                AppendDataBoundItems="True"
                >
                <asp:ListItem Value="0" Text="[No Other Program]"></asp:ListItem>
            </asp:DropDownList>
    </td>
    </tr>
    <tr>
        <td>On Promotional Offers replace this Program to the following Program: </td>
        <td>
            <asp:DropDownList ID="OfferProgram" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                AppendDataBoundItems="True"
                >
                <asp:ListItem Value="0" Text="[No Other Program]"></asp:ListItem>
            </asp:DropDownList>
    </td>
    </tr>
    <tr>
        <td>On Challenges replace this Program to the following Program: </td>
        <td>
            <asp:DropDownList ID="BookListProgram" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
                AppendDataBoundItems="True"
                >
                <asp:ListItem Value="0" Text="[No Other Program]"></asp:ListItem>
            </asp:DropDownList>
    </td>
    </tr>




    <tr>
        <td colspan=2>
            <hr />
            
            <asp:Button ID="btnDelete" runat="server" Text="Delete Program" 
                     CssClass="btn-lg btn-red" onclick="btnDelete_Click"/>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                     CssClass="btn-lg btn-green" onclick="btnCancel_Click" />            
            <hr />
        </td>
        
    </tr>
</table>




<asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsProg" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>

</asp:Content>