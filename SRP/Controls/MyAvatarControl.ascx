<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyAvatarControl.ascx.cs" Inherits="GRA.SRP.Controls.MyAvatarControl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<script type="text/javascript">
    var ASP_components = JSON.parse('<%= json.Serialize(components) %>');
</script>

<div class="row">
    <div class="col-sm-12 hidden-print">
        <span class="h1">
            <asp:Label ID="Label1" runat="server" Text="avatar-title"></asp:Label></span>
    </div>
</div>

<asp:Panel ID="pnlList" runat="server" Visible="true">
    <div class="container">
    <div class="avatar-edit">

        <% for (int i = 0; i < layerCount; i++) { %>
           <img class="avatar-layer" src="/images/Avatars/no_avatar.png" data-component="head" />
        <%  } %>
        
        <% for (int i = 0; i < layerCount; i++) { %>
            <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-left" type="button" data-component="head">&lt;</button>
            <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-right" type="button" data-component="head">&gt;</button>
        <%  } %>
    </div>
        </div>
</asp:Panel>
