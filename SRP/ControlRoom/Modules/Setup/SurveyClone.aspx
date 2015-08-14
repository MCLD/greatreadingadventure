<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="SurveyClone.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyClone" 
%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />
    <hr />
    <asp:Label ID="lblSurvey" runat="server" Text="Label" Font-Bold="True" Font-Size="Large"></asp:Label>
    <asp:Label ID="SID" runat="server" Text="Label" Font-Bold="True" Font-Size="Large" Visible="false"></asp:Label>

<hr />
   <table width="100%" >
        <tr>
            <td nowrap width="110px"> <b> New Name: </b> </td>
            <td colspan="3" >
                
                <asp:TextBox ID="txtNewName" runat="server" Width="500px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAdminName" runat="server" 
                    ControlToValidate="txtNewName" Display="Dynamic" ErrorMessage="New Name is required" 
                    SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>

            <td nowrap> <b> </b> </td>
            <td colspan="3" >
                        
            </td>
        </tr>
        <tr>
            <td colspan="7">
            
            <asp:ImageButton ID="btnCancel44" runat="server" 
                        CausesValidation="false"                         
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Text="Back/Cancel"  Tooltip="Back/Cancel"
                        AlternateText="Back/Cancel" onclick="btnCancel_Click" />
                &nbsp;&nbsp;

                <asp:ImageButton ID="btnPrevious44" runat="server" 
                        CausesValidation="true"                         
                        ImageUrl="~/ControlRoom/Images/clone.png" 
                        Height="25"
                        Text="Clone"  Tooltip="Clone"
                        AlternateText="Clone" onclick="btnClone_Click"   />

            
            </td>
        </tr>
</table>
</asp:Content>