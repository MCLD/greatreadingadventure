<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.About.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="col-xs-9" style="font-size: medium;">
            <p class="margin-1em-top lead">This site is running <a href="http://www.greatreadingadventure.com/" target="_blank">The Great Reading Adventure</a> reading program software.</p>

            <p class="margin-1em-top">Information on using the software can be found in the <a href="http://manual.greatreadingadventure.com/" target="_blank">manual.</a> If you are stuck and need help with the software, users and developers can be found in the <a href="http://forum.greatreadingadventure.com/" target="_blank">forum</a>.</p>

            <p class="margin-1em-top">Version information:</p>
            <table class="table table-striped table-hover table-condensed table-bordered">
                <tr>
                    <th>Assembly</th>
                    <th>Version</th>
                    <th>Status</th>
                </tr>
                <asp:Label runat="server" ID="VersionInformation"></asp:Label>
            </table>
        </div>

        <div class="col-xs-3">
            <asp:Image runat="server" ImageUrl="~/images/gra300.png" />
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
