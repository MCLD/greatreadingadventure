<%@ Page Language="C#" AutoEventWireup="true" 
Title="Summer Reading Program Badge" MasterPageFile="~/Layout/SRP.Master"
CodeBehind="ShareBadge.aspx.cs" Inherits="GRA.SRP.ShareBadge"
 %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
<meta property="og:title" content="I have earned a new badge in the Summer Reading Program"/>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<%

    var BID = 0;
    if (Request["BID"] != null && Request["BID"] != "" )
    {
        int.TryParse(Request["BID"].ToString(), out BID);
    }
    var b = new Badge();
    b.Fetch(BID);
    
%>
<div class="row" style="min-height: 400px;" >

	<div class="span12">
        
           
            <center>
            <h2><%if (b.BID > 0) Response.Write(b.UserName); %></h2>
            
            <br />
            <%if (b.BID > 0) Response.Write("<img border=0 src='/images/badges/" + b.BID.ToString() + ".png'>"); %>
            <br /><br />
            </center>
        
              
        <center>
        <br /><br /><br /><br />

        </center>

	</div> 

</div> 

</asp:Content>