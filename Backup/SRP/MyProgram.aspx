<%@ Page Title="My Program" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyProgram.aspx.cs" Inherits="STG.SRP.MyProgram" %>
<%@ Import Namespace="STG.SRP.DAL" %>
<%@ Register src="~/Controls/ProgramTabs.ascx" tagname="ProgramTabs" tagprefix="uc1" %>
<%@ Register src="~/Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>

<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<!--<uc1:ProgramTabs ID="ProgramTabs1" runat="server" />-->
<!--<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />-->
<center><h1>Welcome, <% = ((Patron)Session["Patron"]).FirstName %></h1></center>
<hr />
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<div class="row">
	<div class="span3">
        <asp:PlaceHolder ID="LeftColumn" runat="server"/>
	</div>

    <div class="span6">
        <asp:PlaceHolder ID="CenterColumn" runat="server"/>
    </div> 

    <div class="span3">
        <asp:PlaceHolder ID="RightColumn" runat="server"/>
	</div>   
</div> 

</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
