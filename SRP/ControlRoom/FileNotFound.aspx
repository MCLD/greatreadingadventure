<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/UnsecuredControl.Master" AutoEventWireup="true" CodeBehind="FileNotFound.aspx.cs" 
Inherits="GRA.SRP.ControlRoom.FileNotFound" 

%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            font-size: 125%;
            font-weight: bold;
        }
    </style>
    <script language="javascript" type="text/javascript">
// <!CDATA[

        function Back_onclick() {
            history.back();
        }

// ]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<p class="style1" style="margin: 20px;">
        <br />
        <br />
        If you believe you reached this page by mistake, please contact the system administrator for more information.</p>
        <br />
    <br />
    <input id="Back" type="button" value="Return to Previous Page" onclick="return Back_onclick()" />
        
</asp:Content>

