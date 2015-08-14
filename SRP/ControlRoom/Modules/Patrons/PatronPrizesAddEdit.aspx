<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="PatronPrizesAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronPrizesAddEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="../../Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronContext ID="pcCtl" runat="server" />

        <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />

    <table>
    <tr>
        <td>Prize Name: </td>
        <td><asp:TextBox ID="PrizeName" runat="server" Text=''  Width="400px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPrizeName" runat="server" 
                    ControlToValidate="PrizeName" Display="Dynamic" ErrorMessage="Prize Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator></td>
    </tr>
    <tr>
        <td colspan=2>
        <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel"    Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" oncommand="btn_Command" />
                        &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" 
                        CausesValidation="True" 
                        CommandName="Add" 
                        ImageUrl="~/ControlRoom/Images/add.png" 
                        Height="25"
                        Text="Add"   Tooltip="Add"
                        AlternateText="Add"  oncommand="btn_Command"
                        /> 
        </td>
    </tr>
    </table>

</asp:Content>

