<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BadgeDetail.aspx.cs" Inherits="STG.SRP.BadgeDetail" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
    .padded { padding: 15px; }
    </style>
</head>
<body background="#FFFFFF" style="background-color: White!important;">
    <form id="form1" runat="server">
    <asp:Label ID="blbBID" runat="server" Text="0" Visible="false"></asp:Label>
    <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
         <h1>Error: Could not find badge</h1>
    </asp:Panel>

    <asp:Panel ID="pnlBadge" runat="server" Visible="false">
    <div style="text-align: center;">
        <h1>
            <asp:Label ID="lblName" runat="server" Text="Label"></asp:Label>
        </h1>
        <asp:Image ID="Image1" runat="server" BorderStyle="Ridge" GenerateEmptyAlternateText="True" AlternateText="Badge" CssClass="padded"/>
    </div>


    <br />

        <asp:Panel ID="pnlEarn" runat="server" Visible="true">
            <h3>You can earn this badge by:</h3>
            <asp:Label ID="lblEarn" runat="server" Text=""></asp:Label>

            <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click" Visible="true"></asp:LinkButton>
        </asp:Panel>


       <asp:Panel ID="pnlEvents" runat="server" Visible="false">
            <h3>You can earn this badge by attending one of these events: </h3>
            <hr />
                <asp:Repeater runat="server" ID="rptr">
                <ItemTemplate>
            
                    <h3><asp:Label ID="Label1" runat="server" Text=""></asp:Label> 
                        <font color="red"><%# Eval("Expired") %></font>
                        <%# ((DateTime)Eval("EventDate")).ToNormalDate() %> <%# Eval("EventTime") %> 

                        <%# Eval("EndDate") == DBNull.Value ? "" : " thru " + ((DateTime)Eval("EndDate")).ToNormalDate().Replace("01/01/1900","")%> 

                        <%# (Eval("EndTime").ToString() != "" ? (Eval("EndDate") == DBNull.Value ? " - " + Eval("EndTime") : Eval("EndTime")) : "")%>

                    </h3>

                    <h3><asp:Label ID="Label2" runat="server" Text=""></asp:Label> <%# Eval("EventTitle") %></h3>
                    <h3><asp:Label ID="Label3" runat="server" Text=""></asp:Label> <%# Eval("Branch")%></h3>

                    <%# (Eval("HTML").ToString().Length > 300 ?  Eval("HTML").ToString().Substring(0, 300) + " ..." : Eval("HTML"))%>
                    <hr />
                
                </ItemTemplate>
            </asp:Repeater>
            
                       



           <asp:Button ID="Button1" runat="server" Text="Go Back" 
               onclick="Button1_Click" />
       </asp:Panel>

    </asp:Panel>


    </form>
</body>
</html>
