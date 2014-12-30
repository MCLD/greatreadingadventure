﻿<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
CodeBehind="PrizesStats.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Reports.PrizesStats" 

%>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="/Scripts/jquery-1.4.1.js" type="text/javascript"></script>
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
<td colspan="2"> 

    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
    <asp:Button ID="btnClear0" runat="server" Text="Export" 
         Width="150px" CSSClass="btn-lg btn-gray" onclick="btnExport_Click"/>

</td>
<td></td>
</tr>
    
</table>
   <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.Programs">
   </asp:ObjectDataSource>

   <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:ObjectDataSource ID="odsDDLibSys" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <br />
    
<div>
    <table cellpadding="5" cellspacing="0" border="1" width="100%">
        <tr style="font-weight: bold; border: 0px solid silver; ">
            <th align="center">Program</th>        
            <td align="center">First Name</td>
            <td align="center">Last Name</td>  
            <td align="center">Username</td>       
            <td align="center">Email</td>       
            <td align="center">Prize Earned</td>
            <td align="center">Prize Redeemed</td>
        </tr>    

        <asp:Repeater runat="server" ID="rptr" >
        <ItemTemplate>
            <tr style="font-weight: normal;">
                <td align="left"><%# Eval("AdminName")%></td>        
                <td align="left"><%# Eval("FirstName")%></td>
                <td align="left"><%# Eval("LastName")%></td>
                <td align="left"><%# Eval("Username")%></td>
                <td align="left"><%# Eval("EmailAddress")%></td>
                <td align="left"><%# Eval("PrizeName")%></td>
                <td align="left"><%# Eval("PrizeRedeemed")%></td>
            </tr> 

        </ItemTemplate>
        </asp:Repeater>


    </table>

</div>
</asp:Content>


