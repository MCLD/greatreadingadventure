<%@ Page Title="Badge Gallery" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Layout/SRP.Master"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.BadgeGallery" %>

<%@ Import Namespace="GRA.SRP.DAL" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-12">
            <span class="h1">
                <asp:Label runat="server" Text="badges-gallery"></asp:Label></span>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12 margin-halfem-top">
            Filter badges:
            <asp:DropDownList ID="CategoryId" runat="server" DataSourceID="odsDDCat" DataTextField="Code" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound">
                <asp:ListItem Value="0" Text="Category"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="AgeGroupId" runat="server" DataSourceID="odsDDAge" DataTextField="Code" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound">
                <asp:ListItem Value="0" Text="Age"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound">
                <asp:ListItem Value="0" Text="All libraries/branches"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="LocationID" runat="server" DataSourceID="odsDDLoc"
                DataTextField="Code" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound">
                <asp:ListItem Value="0" Text="Location"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnFilter" runat="server" Text="badges-filter-button"
                CssClass="btn btn-default btn-xs hidden-print" OnClick="btnFilter_Click" />
        </div>
    </div>
    <div class="row margin-1em-top">
        <div class="col-xs-12">
            <asp:Label ID="NoBadges" runat="server" Text="badges-none-available" Visible="false" CssClass="margin-1em-bottom"></asp:Label>
        </div>
        <asp:Repeater runat="server" ID="rptr" DataSourceID="odsBadges">
            <ItemTemplate>
                <div class="col-xs-6 col-sm-3 col-md-2">
                    <a href='<%# Eval("BID", "~/Badges/Details.aspx?BadgeId={0}") %>'
                        runat="server"
                        onclick='<%# Eval("BID", "return ShowBadgeInfo({0});") %>'
                        class="thumbnail no-underline badge-without-info-height">
                        <div class="text-center caption thumbnail-side-padding" style="padding-left: 2px; padding-right: 2px;"><small><%#Eval("Name") %></small></div>
                        <asp:Image runat="server"
                            ImageUrl='<%# Eval("BID", "~/images/badges/sm_{0}.png") %>'
                            CssClass="center-block" />
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <asp:ObjectDataSource ID="odsBadges" runat="server"
        SelectMethod="GetForGallery"
        TypeName="GRA.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter ControlID="AgeGroupId" DefaultValue="0" Name="Age" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="BranchId" DefaultValue="0" Name="Branch" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="CategoryId" DefaultValue="0" Name="Category" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="LocationID" DefaultValue="0" Name="Location" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>

    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDCat" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Badge Category" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDAge" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Badge Age Group" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDLoc" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Badge Location" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>
