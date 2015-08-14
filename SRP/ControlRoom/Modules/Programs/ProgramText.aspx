<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="ProgramText.aspx.cs" 
    Inherits="GRA.SRP.ControlRoom.Modules.Programs.ProgramText" 
    
    ValidateRequest="False"
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table>
<thead>
    <th colspan="7">
    </th>
</thead>
<tr>


<td><b>Program:</b> </td>
<td>
    <asp:DropDownList ID="ProgramId" runat="server" DataSourceID="odsProg" DataTextField="AdminName" DataValueField="PID" 
        AppendDataBoundItems="True"
        Width="500px"
        >
        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
    </asp:DropDownList>
</td>
<td>
    <asp:Button ID="btnFilter" runat="server" Text="Load Program Text Resources" 
        onclick="btnFilter_Click" CssClass="btn-lg btn-green"/>
        &nbsp;
    <asp:Button ID="btnClear" runat="server" Text="Clear/All" onclick="btnClear_Click"  CssClass="btn-lg btn-blue"
         />

</td>
</tr>
</table>  
    <asp:ObjectDataSource ID="odsProg" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>
    <hr />
    <asp:Panel ID="pnlEdit" runat="server" Visible="False">

    <asp:Button ID="btnSave" runat="server" Text="Save Program Text Resources" 
            onclick="btnSave_Click" CssClass="btn-lg btn-green"/>  &nbsp; 
        <!--<asp:Button ID="btnReload" runat="server" Text="Reload Program Text Resources" 
            onclick="btnReload_Click" CssClass="btn-lg btn-green"/>   &nbsp;-->  &nbsp;  &nbsp;  &nbsp; 
        <asp:Button ID="btnDefaultCSS" runat="server" Text="Load Default Text Resources" onclick="btnDefaultCSS_Click" CssClass="btn-lg btn-purple"
           />
    <br /><br />
    <asp:TextBox ID="txtCSS" runat="server"
        Width="99%" 
    TextMode="MultiLine" Rows="32" Wrap="False"></asp:TextBox>
        </asp:Panel>
</asp:Content>
