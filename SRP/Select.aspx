<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Layout/TSelection.Master" AutoEventWireup="true"
    CodeBehind="Select.aspx.cs" Inherits="STG.SRP.Select" %>
<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
<style>
.ovr
{
	margin-bottom: 0px!important; margin-left: 5px; margin-right:5px;
}

</style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <asp:Label ID="txtMasterDesc" runat="server" Text=""></asp:Label>
    <hr />
    <table border=0>
    <tr>
        <td valign="middle">Select your location: </td>
        <td valign="baseline" >
             <asp:DropDownList ID="ddTenants" runat="server" AutoPostBack="true" AppendDataBoundItems="true"
                onselectedindexchanged="ddTenants_SelectedIndexChanged" CssClass="ovr">
                <asp:ListItem Value="" Text="[Select a Program]"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td valign="middle">
            &nbsp; <asp:Button ID="btnSelProgram" runat="server" Text="Select" CssClass="btn a"
                onclick="btnSelProgram_Click" />
        </td>
   
     
        </tr>
    </table>
    <hr />
    <asp:Label ID="txtTenDesc" runat="server" Text="" ></asp:Label>

</asp:Content>
