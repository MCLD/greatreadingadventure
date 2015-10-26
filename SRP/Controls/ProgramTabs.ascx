<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramTabs.ascx.cs" Inherits="GRA.SRP.Classes.ProgramTabs" %>
<asp:ObjectDataSource ID="odsData" runat="server"
    SelectMethod="GetAllTabs"
    TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>
<ul id="myTab" class="nav nav-pills">
    <asp:Repeater ID="rpt" runat="server" DataSourceID="odsData" OnItemCommand="rpt_ItemCommand">
        <ItemTemplate>
            <li role="presentation" class='<%# (Session["ProgramID"] != null && Session["ProgramID"].ToString()==Eval("PID").ToString() ? "active" : "") %>'>
                <asp:LinkButton runat="server" ID="lnkPgm" CommandName="set" CommandArgument='<%# Eval("PID") %>' Text='<%# Eval("TabName") %>' /></li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
