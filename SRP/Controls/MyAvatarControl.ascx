<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyAvatarControl.ascx.cs" Inherits="GRA.SRP.Controls.MyAvatarControl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<script type="text/javascript">
    var ASP_avatar_components = JSON.parse('<%= json.Serialize(jsAvatarComponents) %>');

    var ASP_avatar_fields = {
        /* [ComponentdID]: [Field] */
        "0": "<%=componentState0.ClientID %>",
        "1": "<%=componentState1.ClientID %>",
        "2": "<%=componentState2.ClientID %>"
    };
        
</script>

<div class="row">
    <div class="col-sm-12 hidden-print">
        <span class="h1">
            <asp:Label ID="Label1" runat="server" Text="avatar-title"></asp:Label></span>
    </div>
</div>

<asp:Panel ID="pnlList" runat="server" Visible="true">



    <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-left" type="button" data-component="0">&lt;</button>
    <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-right" type="button" data-component="0">&gt;</button>
    
    <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-left" type="button" data-component="1">&lt;</button>
    <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-right" type="button" data-component="1">&gt;</button>
    
    <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-left" type="button" data-component="2">&lt;</button>
    <button class="btn btn-primary avatar-layer-btn avatar-layer-btn-right" type="button" data-component="2">&gt;</button>


    <div class="container">
        <div class="avatar-edit">
            <asp:HiddenField id='componentState0' runat="server" />
            <img id="componentImg0" class="avatar-layer" src="/images/Avatars/no_avatar.png"/>
            
            <asp:HiddenField id='componentState1' runat="server" />
            <img id="componentImg1" class="avatar-layer" src="/images/Avatars/no_avatar.png" />

            <asp:HiddenField id='componentState2' runat="server" />
            <img id="componentImg2" class="avatar-layer" src="/images/Avatars/no_avatar.png" />
        </div>
    </div>

    <asp:Button ID="b1" Text="Submit" runat="server" OnClick="SaveButton_Click" />
</asp:Panel>
