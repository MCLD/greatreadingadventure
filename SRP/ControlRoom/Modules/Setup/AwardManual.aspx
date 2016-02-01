<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="AwardManual.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.AwardManual" 

%>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%"  style="border:double 3px #A3C0E8;">
<tr>
    <th colspan="2"><b>Program</b></th>
    <th><b>Library System</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" 
            DataTextField="AdminName" DataValueField="PID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="LibSys" runat="server" DataSourceID="odsDDLibSys" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <th colspan="2"><b>Library/Branch</b></th>
    <th><b>School</b></th>
</tr>
<tr>
    <td colspan="2" width="50%">
        <asp:DropDownList ID="BranchID" runat="server" DataSourceID="odsDDBranch" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="School" runat="server" DataSourceID="odsDDSchool" 
            DataTextField="Code" DataValueField="CID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[All Defined]"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <th colspan="2"><b>Earned points</b></th>
    <th></th>
</tr>
<tr>
    <td colspan="2" align="right">
        <table>
            <tr>
                <td><strong># Points:</strong></td>
                <td><strong>Earned by:</strong></td>            
                <td colspan="3"><strong>Between These Dates:</strong></td>
            </tr>
            <tr>
                <td>
                <asp:TextBox ID="NumPoints" runat="server" Text='' Width="50px"></asp:TextBox>
                <asp:RegularExpressionValidator id="revNumPoints"
                    ControlToValidate="NumPoints"
                    ValidationExpression="\d+"
                    Display="Dynamic"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Must be numeric.</font>"
                    runat="server"
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> *Must be numeric. </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />      
                <asp:RangeValidator ID="rvNumPoints"
                    ControlToValidate="NumPoints"
                    MinimumValue="0"
                    MaximumValue="99999"
                    Display="Dynamic"
                    Type="Integer"
                    EnableClientScript="true"
                    ErrorMessage="<font color='red'>Must be from 0 to 99,999!</font>"
                    runat="server" 
                    Font-Bold="True" Font-Italic="True" 
                    Text="<font color='red'> * Must be from 0 to 99,999! </font>" 
                    EnableTheming="True" 
                    SetFocusOnError="True" />                 
                </td>
                <td>
                    <asp:DropDownList ID="DDPointAwardReason" runat="server">
                        <asp:ListItem Value="-1">[Earned any way possible]</asp:ListItem>
                        <asp:ListItem Value="0">Earned by reading</asp:ListItem>
                        <asp:ListItem Value="1">Earned by attending events</asp:ListItem>
                        <asp:ListItem Value="2">Earned by completing Challenges</asp:ListItem>
                        <asp:ListItem Value="4">Earned by playing games</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                <asp:TextBox ID="StartDate" runat="server" Width="75px" CssClass="datepicker" Text=''></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp;and&nbsp;&nbsp;</td>
                <td>
                    <asp:TextBox ID="EndDate" runat="server" Width="75px" CssClass="datepicker" Text=''></asp:TextBox>                  
                </td>
            </tr>
        
        </table>
    </td>

    <td>
    
    
    </td>
</tr>
<tr>
<td colspan="2"> 

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    
</td>
<td>&nbsp;</td>
</tr>
    
</table>
    <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>
   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:ObjectDataSource ID="odsDDLibSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBadges" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Badge">
    </asp:ObjectDataSource>

    <asp:Panel ID="pnlResults" runat="server" Visible="False">
        <br />
        <asp:Label ID="lblCount" runat="server" Font-Size="Large" ForeColor="#CC0000"></asp:Label>
        <br />
        <br />
        <b>Badge To Award</b><br />
        <asp:DropDownList ID="BadgeID" runat="server" AppendDataBoundItems="True" 
            DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID" 
            Width="97%">
            <asp:ListItem Text="[Select a Badge]" Value="0"></asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button ID="btnAward" runat="server" CSSClass="btn-lg btn-green" 
            onclick="btnAward_Click" Text="Award Badge" Width="150px" />
        <br />
        <br />
        <asp:Label ID="lblAwards" runat="server" Font-Size="Medium" ForeColor="#009933"></asp:Label>
    </asp:Panel>
    <br />
<link rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/themes/smoothness/jquery-ui.css" />
<script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>
<script language='javascript' type='text/javascript'>
    $(function () {
        $(".datepicker").datepicker();
    });
    
</script>

</asp:Content>
