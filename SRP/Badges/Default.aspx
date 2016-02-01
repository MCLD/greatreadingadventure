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
        <div class="col-sm-12 margin-halfem-top form-inline">
            <label>Filter badges:</label>
            <asp:DropDownList ID="CategoryId" runat="server" DataSourceID="odsDDCat" DataTextField="Description" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound" CssClass="form-control margin-1em-right">
                <asp:ListItem Value="0" Text="Category"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="AgeGroupId" runat="server" DataSourceID="odsDDAge" DataTextField="Description" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound" CssClass="form-control margin-1em-right">
                <asp:ListItem Value="0" Text="Age"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Description" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound" CssClass="form-control margin-1em-right">
                <asp:ListItem Value="0" Text="All libraries/branches"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="LocationID" runat="server" DataSourceID="odsDDLoc"
                DataTextField="Description" DataValueField="CID"
                AppendDataBoundItems="True" Width="200px" OnDataBound="dd_DataBound" CssClass="form-control margin-1em-right">
                <asp:ListItem Value="0" Text="Location"></asp:ListItem>
            </asp:DropDownList>
            <div class="margin-halfem-top margin-halfem-bottom" style="display: inline-block;">
                <asp:HyperLink runat="server" Id="myBadgesButton" NavigateUrl="~/Badges/MyBadges.aspx"
                    CssClass="btn btn-default btn-sm hidden-print margin-1em-right" OnClick="btnFilter_Click"><asp:Label runat="server" Text="badges-mybadges-button"></asp:Label></asp:HyperLink>
                <asp:Button ID="btnFilter" runat="server" Text="badges-filter-button"
                    CssClass="btn btn-default btn-sm hidden-print margin-1em-right" OnClick="btnFilter_Click" />
                <asp:Button ID="btnClear" runat="server" Text="badges-filter-clear-button"
                    CssClass="btn btn-default btn-sm hidden-print" OnClick="btnClear_Click" />
            </div>
        </div>
    </div>
    <div class="row margin-1em-top">
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
            <FooterTemplate>
                <div class="col-xs-12" runat="server" visible="<%#rptr.Items.Count == 0 %>">
                    <strong><%=this.NoneAvailableText %></strong>
                </div>
            </FooterTemplate>
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
