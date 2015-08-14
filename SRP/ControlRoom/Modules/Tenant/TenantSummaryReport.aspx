<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="TenantSummaryReport.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Tenant.TenantSummaryReport" 

%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", {
            packages: ["corechart"]
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%"  style="border:double 3px #A3C0E8;">


<tr>
<td>

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    <asp:Button ID="btnExport" runat="server" Text="Export" 
         Width="150px" CSSClass="btn-lg btn-gray" onclick="btnExport_Click"/>
         </td>
<td colspan="2"> 
    <asp:CheckBox ID="chk" runat="server" Text="Include Rollup Summary"></asp:CheckBox><br />(Note: this is a massive report.  Inlcuding the rollup summary will double the tun time for the report)
</td> 

</tr>
    
</table>


    <br />

<div style='background-color: white; min-height: 400px; border: 0px solid red;'>
    <table cellpadding="5" cellspacing="0" border="1" width="100%" style='background-color: white; min-height:400px;'>
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <th align="left" >Organization</th>        
            <td align="left" ># Programs</td>
            <td align="left" ># Patrons</td>
            <td align="left" ># Finishers</td>  
            <td align="left" ># Badges</td>  
            <td align="left" >Male Participation</td>  
            <td align="left" >Female Participation</td>  
            <td align="left" ># Reading Points</td>  
            <td align="left" ># Total Points</td>  
            <td align="left" ># Reading Minutes</td>  
      
        </tr>    

        <asp:Repeater runat="server" ID="rptr" >
        <ItemTemplate>
            <tr style="font-weight: normal;">
                <td align="left"><%# Eval("Organization")%></td>      
                <td align="left"><%# Eval("# Programs")%></td>     
                <td align="right"><%# Eval("# Patrons")%></td>
                <td align="right"><%# Eval("# Finishers")%></td>  
                <td align="right"><%# Eval("# Badges")%></td>
                <td align="right"><%# Eval("Male Participation")%></td>  
                <td align="right"><%# Eval("Female Participation")%></td> 
                <td align="right"><%# Eval("# Reading Points")%></td> 
                <td align="right"><%# Eval("# Total Points")%></td> 
                <td align="right"><%# Eval("# Reading Minutes")%></td> 
                
            </tr> 

        </ItemTemplate>
        </asp:Repeater>

<tr style="font-weight: bold; border: 0px solid white; "> <td colspan=24 height="100%"></td></tr>

    </table>

</div>

</asp:Content>


