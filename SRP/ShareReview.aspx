<%@ Page Language="C#" AutoEventWireup="true" 
Title="Summer Reading Program review" MasterPageFile="~/Layout/SRP.Master"
CodeBehind="ShareReview.aspx.cs" Inherits="GRA.SRP.ShareReview"
 %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
<meta property="og:title" content="I have reviewed a book for the Summer Reading Program"/>
<meta property="og:image" content='<%=Request.Url.Scheme+"://"+Request.Url.Authority %>/images/books.png' />
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<%

    var PRID = 0;
    if (Request["ID"] != null && Request["ID"] != "")
    {
        int.TryParse(Request["ID"].ToString(), out PRID);
    }
    var pr = PatronReview.FetchObject(PRID);
%>
<div class="row" style="min-height: 400px;" >

	<div class="span12">
        
           <% = FormatReading(pr.Author, pr.Title, pr.Review) %>
                     
        <center>
        <br /><br /><br /><br />

        </center>

	</div> 

</div> 

</asp:Content>