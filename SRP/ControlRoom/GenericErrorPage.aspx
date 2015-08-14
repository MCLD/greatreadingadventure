<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/UnsecuredControl.Master" AutoEventWireup="true" CodeBehind="GenericErrorPage.aspx.cs" 
Inherits="GRA.SRP.ControlRoom.GenericErrorPage" 

%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        // <!CDATA[

        function Back_onclick() {
            history.back();
        }

        // ]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>
        <b>An unexpected Error has occured</b>
        </h2>
    <asp:Label ID="uxExceptionMessage" runat="server" Text=""></asp:Label>
    <h2>
        <b>What will be affected</b></h2>
    <asp:Label ID="uxErrorAffected" runat="server" Text="Your last action was not successfully completed."></asp:Label>
    <h2>
        <b>What you can do from here</b></h2>
    <asp:Label ID="uxErrorWhatToDo" runat="server" Text="Go back up to the previous page and verify that all data input is correct."></asp:Label>
    <h2>
        <b>Support Information</b></h2>
    <asp:Label ID="uxSupport" runat="server" Text="If you need assistance, please contact the Help Desk."></asp:Label>
    <h2>
        <b>Stack Trace</b></h2>
    <asp:Label ID="uxStackTrace" runat="server" Text="N/A"></asp:Label>
    
        <br />
    <br />
    <input id="Back" type="button" value="Return to Previous Page" onclick="return Back_onclick()" />

</asp:Content>
