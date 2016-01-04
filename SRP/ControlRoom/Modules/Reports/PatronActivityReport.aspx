<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="PatronActivityReport.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.PatronActivityReport" 

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
<td>

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    <asp:Button ID="btnClear0" runat="server" Text="Export" 
         Width="150px" CSSClass="btn-lg btn-gray" onclick="btnExport_Click"/>
         </td>
<td colspan="2"> 

    &nbsp;</td> 

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

    <br />

<div style='background-color: white; min-height: 400px; border: 0px solid red;'>
    <table cellpadding="5" cellspacing="0" border="1" width="100%" style='background-color: white; min-height:400px;'>
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <th align="left" >Program</th>        
            <td align="left" >Username</td>
            <td align="left" >Patron Name</td>
            <td align="left" >Library District</td>  
            <td align="left" >Library</td>  
            <td align="left" >School District</td>  
            <td align="left" >School</td>  
            <td align="left" >Age</td>  
            <td align="left" >Grade</td>  
            <td align="left" >Score1</td>  
            <td align="left" >Score1 Date</td>  
            <td align="left" >Score2</td>  
            <td align="left" >Score2 Date</td>  
            <td align="left" >Difference</td>  
            <td align="left" ># Points For Reading</td>  
            <td align="left" ># Points For Events</td>  
            <td align="left" ># Points For Games</td>  
            <td align="left" ># Points For Challenges</td>  
            <td align="left" ># Times Logged Reading</td>  
            <td align="left" ># Events Attended</td>  
            <td align="left" ># Times Logged Games</td>  
            <td align="left" ># Challenges Completed</td>  
            <td align="left" ># Badges Earned</td>  
            <td align="left" ># Minigames Played</td>  
        </tr>    

        <asp:Repeater runat="server" ID="rptr" >
        <ItemTemplate>
            <tr style="font-weight: normal;">
                <td align="left"><%# Eval("Program")%></td>      
                <td align="left"><%# Eval("Username")%></td>   
                <td align="left"><%# Eval("FirstName")%> <%# Eval("LastName")%></td>     
                <td align="right"><%# Eval("LibraryDistrict")%></td>
                <td align="right"><%# Eval("Library")%></td>  
                <td align="right"><%# Eval("SchoolDistrict")%></td>
                <td align="right"><%# Eval("School")%></td>  
                <td align="right"><%# Eval("Age")%></td> 
                <td align="right"><%# Eval("SchoolGrade")%></td> 
                <td align="right"><%# Eval("Score1")%></td> 
                <td align="right"><%# Eval("Score1Date")%></td> 
                <td align="right"><%# Eval("Score2")%></td> 
                <td align="right"><%# Eval("Score2Date")%></td> 
                <td align="right"><%# Eval("ScoreDifference")%></td> 
                <td align="right"><%# Eval("# Points For Reading")%></td> 
                <td align="right"><%# Eval("# Points For Events")%></td> 
                <td align="right"><%# Eval("# Points For Games")%></td> 
                <td align="right"><%# Eval("# Points For Book Lists")%></td> 
                <td align="right"><%# Eval("# Times Logged Reading")%></td> 
                <td align="right"><%# Eval("# Events Attended")%></td> 
                <td align="right"><%# Eval("# Times Logged Games")%></td> 
                <td align="right"><%# Eval("# Book Lists Completed")%></td> 
                <td align="right"><%# Eval("# Badges Earned")%></td> 
                <td align="right"><%# Eval("# Minigames Played")%></td> 
                           
                
                
            </tr> 

        </ItemTemplate>
        </asp:Repeater>

<tr style="font-weight: bold; border: 0px solid white; "> <td colspan=24 height="100%"></td></tr>

    </table>

</div>

</asp:Content>


