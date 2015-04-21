<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramTabs.ascx.cs" Inherits="STG.SRP.Classes.ProgramTabs" %>
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAllTabs" 
        TypeName="STG.SRP.DAL.Programs">
	</asp:ObjectDataSource>

<div class="row">
	<div class="span12">
		<ul id="myTab" class="nav nav-tabs">
            <asp:Repeater ID="rpt" runat="server" DataSourceID="odsData" 
                onitemcommand="rpt_ItemCommand">
                <ItemTemplate>
                        <li class='<%# (Session["ProgramID"] != null && Session["ProgramID"].ToString()==Eval("PID").ToString() ? "active" : "") %>'><asp:LinkButton runat="server" id="lnkPgm" CommandName="set" CommandArgument='<%# Eval("PID") %>' Text='<%# Eval("TabName") %>' /></li>
                </ItemTemplate>
            </asp:Repeater>
		</ul>
	</div>
</div>


